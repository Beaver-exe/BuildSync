using BuildSync.Services;
using BuildSync.DTOs.Member;

namespace BuildSync.Endpoints;

public static class Members
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectId}/members")
        .WithTags("Members")
        .RequireAuthorization();

        group.MapPost("/", async (MemberService memb, Guid projectId, AddMemberRequest request) =>
        {
            var success = await memb.AddMemberAsync(projectId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapPatch("/{userId}", async (MemberService memb, Guid projectId, Guid userId, EditMemberRequest request) =>
        {
            var success = await memb.EditMemberStatus(projectId, userId, request);

            if (!success)
            {
                return Results.BadRequest("Failed to add user to project");
            }

            return Results.Ok();
        });

        group.MapDelete("/{userId}", async (MemberService memb, Guid projectId, Guid userId) =>
        {
            var success = await memb.RemoveMemberStatusAsync(projectId, userId);

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