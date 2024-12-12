using System.Security.Claims;
using FluentValidation;
using Lab1.Auth.Model;
using Lab1.Data;
using Lab1.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;

namespace Lab1.EndPoints;

public static class Endpoints
{
    public static void AddMealApi(this WebApplication app)
    {
        var mealsGroup = app. MapGroup (prefix: "/api").WithValidationFilter();

        mealsGroup.MapGet("meals", async (ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            return (await dbContext.Meals.ToListAsync(cancellationToken))
                .Select(o => new MealDto(o.Id, o.Name, o.Description, o.CreationDate));
        });

        mealsGroup.MapGet("meals/{mealId}", async (int mealId, ForumDbContext dbContext) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if(meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            return Results.Ok(new MealDto(meal.Id, meal.Name, meal.Description, meal.CreationDate) );
        });

        mealsGroup.MapPost ("meals", [Authorize(Roles = ForumRoles.Admin)]async ([Validate]CreateMealDto CreateMealDto, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        {
            var meal = new Meal()
            {
                Name = CreateMealDto.Name,
                Description = CreateMealDto.Description,
                CreationDate = DateTime.UtcNow,
                UserId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            
            dbContext.Meals.Add(meal);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/meals/{meal.Id}",
                new MealDto(meal.Id, meal.Name, meal.Description, meal.CreationDate));
        });

        mealsGroup.MapPut("meals/{mealId}", [Authorize(Roles = ForumRoles.Admin)]async (int mealId, ForumDbContext dbContext, [Validate]UpdateMealDto dto, IHttpContextAccessor httpContext) =>
        {    
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if(meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin))
            {
                return Results.Forbid();
            }
            
            meal.Description = dto.Description;
            dbContext.Update(meal);
            await dbContext.SaveChangesAsync();
            
            return Results.Ok(new MealDto(meal.Id, meal.Name, meal.Description, meal.CreationDate) );
        });

        mealsGroup.MapDelete("meals/{mealId}", [Authorize(Roles = ForumRoles.Admin)]async (int mealId, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        {    
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if(meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin))
            {
                return Results.Forbid();
            }
            dbContext.Remove(meal);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    public static void AddRecipieApi(this WebApplication app)
    {
        //RECIPIE
        var recipieGroup = app.MapGroup (prefix: "/api/meals/{mealId}").WithValidationFilter();

        recipieGroup.MapGet("recipies", async (int mealId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipies = await dbContext.Recipies
                .Where(r => r.Meal.Id == mealId)
                .ToListAsync(cancellationToken);

            return Results.Ok(recipies.Select(r => new RecipieDto(r.Id, r.Name, r.Description, r.CreationDate)));
        });

        recipieGroup.MapGet("recipies/{recipieId}", async (int mealId, int recipieId, ForumDbContext dbContext) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            
            return Results.Ok(new RecipieDto(recipie.Id, recipie.Name, recipie.Description, recipie.CreationDate) );
        });

        recipieGroup.MapPost("recipies", [Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, [Validate] CreateRecipieDto createRecipieDto, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");

            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            var recipie = new Recipie()
            {
                Name = createRecipieDto.Name,
                Description = createRecipieDto.Description,
                CreationDate = DateTime.UtcNow,
                Meal = meal,
                UserId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            
            dbContext.Recipies.Add(recipie);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/meals/{meal.Id}/recipies/{recipie.Id}",
                new RecipieDto(recipie.Id, recipie.Name, recipie.Description, recipie.CreationDate));
        });

        recipieGroup.MapPut("recipies/{recipieId}",[Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, int recipieId, ForumDbContext dbContext, [Validate]UpdateRecipieDto dto, IHttpContextAccessor httpContext) =>
        {    
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin) && httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != recipie.UserId)
            {
                return Results.Forbid();
            }
            
            recipie.Description = dto.Description;
            dbContext.Update(recipie);
            await dbContext.SaveChangesAsync();
            
            return Results.Ok(new RecipieDto(recipie.Id, recipie.Name, recipie.Description, recipie.CreationDate) );
        });

        recipieGroup.MapDelete("recipies/{recipieId}",[Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, int recipieId, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        { 
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin) && httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != recipie.UserId)
            {
                return Results.Forbid();
            }
            
            dbContext.Remove(recipie);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    public static void AddCommentApi(this WebApplication app)
    {
        //COMMENT
        var commentGroup = app.MapGroup (prefix: "/api/meals/{mealId}/recipies/{recipieid}").WithValidationFilter();

        commentGroup.MapGet("comments", async (int mealId, int recipieId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            
            var comments = await dbContext.Comments
                .Where(c => c.Recipie.Id == recipieId)
                .ToListAsync(cancellationToken);

            return Results.Ok(comments.Select(c => new CommentDto(c.Id, c.Content, c.CreationDate)));
        });

        commentGroup.MapGet("comments/{commentId}", async (int mealId, int recipieId, int commentId, ForumDbContext dbContext) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.Recipie.Id == recipieId);
            if(comment == null)
                return Results.NotFound($"Comment with ID {commentId} not found for Recipie ID {recipieId}");

            return Results.Ok(new CommentDto(comment.Id, comment.Content, comment.CreationDate));
        });


        commentGroup.MapPost("comments", [Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, int recipieId, [Validate] CreateCommentDto CreateCommentDto, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        {
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Meal.Id == mealId && r.Id == recipieId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            var comment = new Comment()
            {
                Content = CreateCommentDto.Content,
                CreationDate = DateTime.UtcNow,
                Recipie = recipie,
                UserId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/meals/{meal.Id}/recipies/{recipie.Id}/comments/{comment.Id}",
                new CommentDto(comment.Id, comment.Content, comment.CreationDate));
        });

        commentGroup.MapPut("comments/{commentId}", [Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, int recipieId, int commentId, ForumDbContext dbContext, [Validate]UpdateCommentDto dto, IHttpContextAccessor httpContext) =>
        {    
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
            
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
            
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.Recipie.Id == recipieId);
            if(comment == null)
                return Results.NotFound($"Comment with ID {commentId} not found for Recipie ID {recipieId}");
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin) && httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != comment.UserId)
            {
                return Results.Forbid();
            }
            
            comment.Content = dto.Content;
            dbContext.Update(comment);
            await dbContext.SaveChangesAsync();
            
            return Results.Ok(new CommentDto(comment.Id, comment.Content, comment.CreationDate) );
        });

        commentGroup.MapDelete("comments/{commentId}", [Authorize(Roles = ForumRoles.ForumUser)]async (int mealId, int recipieId, int commentId, ForumDbContext dbContext, IHttpContextAccessor httpContext) =>
        {    
            var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == mealId);
            if (meal == null)
                return Results.NotFound($"Meal with ID {mealId} not found");
                
            var recipie = await dbContext.Recipies.FirstOrDefaultAsync(r => r.Id == recipieId && r.Meal.Id == mealId);
            if (recipie == null)
                return Results.NotFound($"Recipie with ID {recipieId} not found for Meal ID {mealId}");
                
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.Recipie.Id == recipieId);
            if(comment == null)
                return Results.NotFound($"Comment with ID {commentId} not found for Recipie ID {recipieId}");
            var userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user?.ForceRelogin == true)
            {
                return Results.Unauthorized();
            }
            if(!httpContext.HttpContext.User.IsInRole(ForumRoles.Admin) && httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != comment.UserId)
            {
                return Results.Forbid();
            }
            
            dbContext.Remove(comment);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}



//FOR MEAL
public record CreateMealDto(string Name, string Description);
public record UpdateMealDto(string Description);
public class CreateMealDtoValidator : AbstractValidator<CreateMealDto>
{
    public CreateMealDtoValidator()
    {
        RuleFor(m => m.Name).NotEmpty().NotNull().Length(min: 1, max: 20);
        RuleFor(m => m.Description).NotEmpty().NotNull().Length(min: 1, max: 100);
    }
}
public class UpdateMealDtoValidator : AbstractValidator<UpdateMealDto>
{
    public UpdateMealDtoValidator()
    {
        RuleFor(m => m.Description).NotEmpty().NotNull().Length(min: 1, max: 100);
    }
}

//FOR RECIPIE
public record CreateRecipieDto(string Name, string Description);
public record UpdateRecipieDto(string Description);
public class CreateRecipieDtoValidator : AbstractValidator<CreateRecipieDto>
{
    public CreateRecipieDtoValidator()
    {
        RuleFor(r => r.Name).NotEmpty().NotNull().Length(min: 1, max: 20);
        RuleFor(r => r.Description).NotEmpty().NotNull().Length(min: 1, max: 3000);
    }
}
public class UpdateRecipieDtoValidator : AbstractValidator<UpdateRecipieDto>
{
    public UpdateRecipieDtoValidator()
    {
        RuleFor(r => r.Description).NotEmpty().NotNull().Length(min: 1, max: 3000);
    }
}

//FOR COMMENT
public record CreateCommentDto(string Content);
public record UpdateCommentDto(string Content);
public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(c => c.Content).NotEmpty().NotNull().Length(min: 1, max: 200);
    }
}
public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(c => c.Content).NotEmpty().NotNull().Length(min: 1, max: 200);
    }
}