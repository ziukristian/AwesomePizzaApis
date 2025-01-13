using AwesomePizzaApis.Model;
using AwesomePizzaApis.Services;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomePizzaApis;

public record OrderCreateRequest(string PizzaType, string CustomerEmail);

public class OrderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // [GET] /orders
        app.MapGet(
            "orders",
            async (AppDbContext _context, IOrderService _orderService) =>
            {
                List<Order> orders = await _context.Orders.ToListAsync();

                return Results.Ok(orders);
            }
        );

        // [GET] /orders/{orderStatus}
        app.MapGet(
            "orders/{orderStatus}",
            async (AppDbContext _context, IOrderService _orderService, string orderStatus) =>
            {
                if (!Enum.TryParse<OrderStatus>(orderStatus, out var parsedStatus))
                {
                    return Results.BadRequest("Invalid status");
                }

                List<Order> orders = await _context
                    .Orders.Where(o => o.Status == parsedStatus)
                    .ToListAsync();

                var organizedOrders = _orderService.OrganizeOrders(orders);

                return Results.Ok(organizedOrders);
            }
        );

        // [GET] /orders/{orderId}/status
        app.MapGet(
            "orders/{orderId}/status",
            async (AppDbContext _context, int orderId) =>
            {
                Order? order = await _context
                    .Orders.Include(o => o.Status)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return Results.NotFound("Order not found");
                }

                return Results.Ok(order.Status.ToString());
            }
        );

        // [GET] /orders/next
        app.MapGet(
            "orders/next",
            async (AppDbContext _context, IOrderService _orderService) =>
            {
                List<Order> orders = await _context.Orders.ToListAsync();

                Order? nextOrder = _orderService.FindNextOrder(orders);

                if (nextOrder == null)
                {
                    return Results.NotFound("No orders found");
                }

                return Results.Ok(nextOrder);
            }
        );

        // [POST] /orders
        app.MapPost(
            "orders",
            async (AppDbContext _context, OrderCreateRequest orderCreateRequest) =>
            {
                Order order =
                    new()
                    {
                        PizzaType = orderCreateRequest.PizzaType,
                        CustomerEmail = orderCreateRequest.CustomerEmail,
                        Status = OrderStatus.Created,
                    };

                await _context.Orders.AddAsync(order);

                await _context.SaveChangesAsync();

                return Results.Created($"/orders/{order.Id}", order);
            }
        );

        // [PATCH] /orders/{orderId}/status
        app.MapPatch(
            "orders/{orderId}/status",
            async (AppDbContext _context, int orderId, [FromBody] string newStatus) =>
            {
                if (!Enum.TryParse<OrderStatus>(newStatus, out var parsedStatus))
                {
                    return Results.BadRequest("Invalid status");
                }

                if (parsedStatus == OrderStatus.InProgress)
                {
                    Order? orderInProgress = await _context.Orders.FirstOrDefaultAsync(o =>
                        o.Status == OrderStatus.InProgress
                    );

                    if (orderInProgress != null)
                    {
                        return Results.BadRequest("An order is already in progres");
                    }
                }

                Order? order = await _context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    return Results.NotFound();
                }

                order.Status = parsedStatus;
                await _context.SaveChangesAsync();
                return Results.Ok();
            }
        );

        // [DELETE] /orders/{orderId}
        app.MapDelete(
            "orders/{orderId}",
            async (AppDbContext _context, int orderId) =>
            {
                Order? order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Results.Ok();
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Results.Ok();
            }
        );
    }
}
