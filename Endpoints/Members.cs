using BuildSync.Services;
using BuildSync.DTOs.Member;

namespace BuildSync.Endpoints;

public static class Members
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectId}/members").RequireAuthorization();

        group.MapPost("/", async (MemberService memb, int projectId, MemberRequest request) =>
        {
            var success = await memb.AddMemberAsync(projectId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapPatch("/", async (MemberService memb, int projectId, MemberRequest request) =>
        {
            var success = await memb.EditMemberStatus(projectId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapDelete("/", async (MemberService memb, int projectId, RemoveMemberRequest request) =>
        {
            var success = await memb.RemoveMemberStatusAsync(projectId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to remove user from project");
            }

            return Results.Ok();
        });

        group.MapDelete("/me", async (MemberService memb, int projectId, RemoveMemberRequest request) =>
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