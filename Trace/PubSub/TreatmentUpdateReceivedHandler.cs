using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Data.Models.Pubsub;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class TreatmentUpdateReceivedHandler : INotificationHandler<LowTrustUserTreatmentUpdateReceived>
{
    private readonly AppDbContext _ctx;

    public TreatmentUpdateReceivedHandler(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(LowTrustUserTreatmentUpdateReceived notification, CancellationToken cancellationToken)
    {
        var entity = new ModeratorAction
        {
            Action = "low_trust_user_treatment_update",
            Timestamp = notification.ReceivedAt,
            ChannelId = notification.Topic.ChannelId,
            InitiatorId = notification.TreatmentUpdate.UpdatedBy.Id,
            InitiatorName = notification.TreatmentUpdate.UpdatedBy.Login,
            // notification.TreatmentUpdate.UpdatedBy.Name
            TargetId = notification.TreatmentUpdate.TargetUserId,
            TargetName = notification.TreatmentUpdate.TargetUser,
            LowTrustUserTreatment = notification.TreatmentUpdate.Treatment,
            LowTrustUserTypes = notification.TreatmentUpdate.Types.ToList(),
            LowTrustUserBanEvasionEvaluation = notification.TreatmentUpdate.BanEvasionEvaluation,
            UpdatedAt = notification.TreatmentUpdate.UpdatedAt,
        };

        _ctx.ModeratorActions.Add(entity);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
