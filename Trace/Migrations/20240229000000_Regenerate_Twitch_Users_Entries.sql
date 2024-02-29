TRUNCATE TABLE twitch.users RESTART IDENTITY;

INSERT INTO twitch.users (id, login, first_seen, last_seen)
WITH entries AS (
    SELECT timestamp,
           author_id AS id,
           author_login AS login
    FROM tmi.messages
    UNION ALL
    SELECT *
    FROM (
        SELECT timestamp,
               target_id AS id,
               target_name AS login
        FROM pubsub.moderator_actions
        UNION ALL
        SELECT timestamp,
               initiator_id,
               initiator_name
        FROM pubsub.moderator_actions
    ) AS actions_entries
    WHERE login ~ '^[a-z0-9_]+$'
    AND id IS NOT NULL
),
marked_changes AS (
    SELECT
        timestamp,
        id,
        login,
        CASE
            WHEN LAG(login) OVER (PARTITION BY id ORDER BY timestamp) = login THEN 0
            ELSE 1
        END AS login_changed
    FROM entries
),
cumulative_groups AS (
    SELECT
        timestamp,
        id,
        login,
        SUM(login_changed) OVER (PARTITION BY id ORDER BY timestamp) AS group_id
    FROM marked_changes
),
grouped AS (
    SELECT
        id,
        login,
        MIN(timestamp) AS first,
        MAX(timestamp) AS last,
        group_id
    FROM cumulative_groups
    GROUP BY id, login, group_id
)
SELECT
    id,
    login,
    first,
    last
FROM grouped
ORDER BY id, first, login;
