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
    
    }
}