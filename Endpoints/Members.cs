using BuildSync.Services;
using BuildSync.DTOs.Member;

namespace BuildSync.Endpoints;

public static class Members
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectId}/members").RequireAuthorization();

        group.MapPost("/", async (MemberService memb, Guid projectId, AddMemberRequest request) =>
        {
            var success = await memb.AddMemberAsync(projectId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapPatch("/{userGuid}", async (MemberService memb, Guid projectId, Guid userGuid, EditMemberRequest request) =>
        {
            var success = await memb.EditMemberStatus(projectId, userGuid, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapDelete("/{userGuid}", async (MemberService memb, Guid projectId, Guid userGuid) =>
        {
            var success = await memb.RemoveMemberStatusAsync(projectId, userGuid);

            if (!success)
            {
                return Results.BadRequest("Failed to remove user from project");
            }

            return Results.Ok();
        });

        group.MapDelete("/me", async (MemberService memb, Guid projectId) =>
        {
            var success = await memb.LeaveProjectAsync(projectId);

            if (!success)
            {
                return Results.BadRequest("Failed to leave project");
            }

            return Results.Ok();
        });
    }
}