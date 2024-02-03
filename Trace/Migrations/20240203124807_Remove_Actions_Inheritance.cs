using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Actions_Inheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discriminator",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set duration = slow_duration
                where action = 'slow';
                """);

            migrationBuilder.DropColumn(
                name: "slow_duration",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set target_name = targeted_moderator_action_target_name
                where action in ('ban', 'unban', 'delete', 'timeout', 'untimeout', 'mod', 'unmod', 'vip_added', 'approve_unban_request', 'deny_unban_request', 'shoutout');
                """);

            migrationBuilder.DropColumn(
                name: "targeted_moderator_action_target_name",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set reason = timeout_reason
                where action = 'timeout';
                """);

            migrationBuilder.DropColumn(
                name: "timeout_reason",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set duration = timeout_duration
                where action = 'timeout';
                """);

            migrationBuilder.DropColumn(
                name: "timeout_duration",
                schema: "pubsub",
                table: "moderator_actions");

            migrationBuilder.RenameColumn(
                name: "text",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "term_text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "term_text",
                schema: "pubsub",
                table: "moderator_actions",
                newName: "text");

            // TODO: migrationBuilder.Sql
            migrationBuilder.AddColumn<string>(
                name: "discriminator",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set discriminator =
                        case action
                            when 'ban' then 'Ban'
                            when 'unban' then 'TargetedModeratorAction'
                            when 'clear' then 'ModeratorAction'
                            when 'delete' then 'Delete'
                            when 'emote_only' then 'ModeratorAction'
                            when 'emote_only_off' then 'ModeratorAction'
                            when 'followers' then 'Followers'
                            when 'followersoff' then 'ModeratorAction'
                            when 'raid' then 'Raid'
                            when 'unraid' then 'ModeratorAction'
                            when 'slowoff' then 'ModeratorAction'
                            when 'slow' then 'Slow'
                            when 'subscribers' then 'ModeratorAction'
                            when 'subscribers_off' then 'ModeratorAction'
                            when 'r9kbeta' then 'ModeratorAction'
                            when 'r9kbetaoff' then 'ModeratorAction'
                            when 'untimeout' then 'TargetedModeratorAction'
                            when 'mod' then 'TargetedModeratorAction'
                            when 'unmod' then 'TargetedModeratorAction'
                            when 'vip_added' then 'TargetedModeratorAction'
                            when 'approve_unban_request' then 'UnbanRequestAction'
                            when 'deny_unban_request' then 'UnbanRequestAction'
                            when 'add_blocked_term' then 'TermAction'
                            when 'delete_blocked_term' then 'TermAction'
                            when 'add_permitted_term' then 'TermAction'
                            when 'delete_permitted_term' then 'TermAction'
                            when 'shoutout' then 'TargetedModeratorAction'
                            when 'timeout' then 'Timeout'
                            end;
                """);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "slow_duration",
                schema: "pubsub",
                table: "moderator_actions",
                type: "interval",
                nullable: true);

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set slow_duration = duration,
                    duration = null
                where action = 'slow';
                """);

            migrationBuilder.AddColumn<string>(
                name: "targeted_moderator_action_target_name",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set targeted_moderator_action_target_name = target_name,
                    target_name = null
                where action in ('ban', 'unban', 'delete', 'timeout', 'untimeout', 'mod', 'unmod', 'vip_added', 'approve_unban_request', 'deny_unban_request', 'shoutout');
                """);

            migrationBuilder.AddColumn<string>(
                name: "timeout_reason",
                schema: "pubsub",
                table: "moderator_actions",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set timeout_reason = reason,
                    reason = null
                where action = 'timeout';
                """);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "timeout_duration",
                schema: "pubsub",
                table: "moderator_actions",
                type: "interval",
                nullable: true);

            migrationBuilder.Sql(
                """
                update pubsub.moderator_actions
                set timeout_duration = duration,
                    duration = null
                where action = 'timeout';
                """);
        }
    }
}
