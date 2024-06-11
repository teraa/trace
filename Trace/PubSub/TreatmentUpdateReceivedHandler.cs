using JetBrains.Annotations;
using MediatR;
using Teraa.Twitch.PubSub.Notifications;
using Trace.Data;
using Trace.Data.Models.Pubsub;
using Trace.Features.Users;

namespace Trace.PubSub;

[UsedImplicitly]
public sealed class TreatmentUpdateReceivedHandler : INotificationHandler<LowTrustUserTreatmentUpdateReceived>
{
    private readonly AppDbContext _ctx;
    private readonly UpdateUser.Handler _updateUserHandler;

    public TreatmentUpdateReceivedHandler(AppDbContext ctx, UpdateUser.Handler updateUserHandler)
    {
        _ctx = ctx;
        _updateUserHandler = updateUserHandler;
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

        await _updateUserHandler.HandleAsync(new UpdateUser.Command(
            notification.TreatmentUpdate.UpdatedBy.Id,
            notification.TreatmentUpdate.UpdatedBy.Login,
            notification.ReceivedAt), cancellationToken);

        await _updateUserHandler.HandleAsync(new UpdateUser.Command(
            notification.TreatmentUpdate.TargetUserId,
            notification.TreatmentUpdate.TargetUser,
            notification.ReceivedAt), cancellationToken);
    }
}
