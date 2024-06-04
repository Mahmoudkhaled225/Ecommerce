using System.Security.Claims;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using AutoMapper;
using C_ServiceLayer.Abstractions;
using D_PresentationLayer.Dtos.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;

public class OrderController : BaseController
{
    
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentService _paymentService;

    public OrderController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> userManager, ICartRepository cartRepository, IPaymentService paymentService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _cartRepository = cartRepository;
        _paymentService = paymentService;
    }

    [HttpPost("Add")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> AddOrder([FromBody] AddOrder dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest("Invalid token");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound("User not found");

        var cart = await _cartRepository.GetCart(user.Id);
        if (cart is null)
            return NotFound("Cart is not found so add products to cart first");

        if (cart.CartItems.Count is 0)
            return BadRequest("Cart is empty so add products to cart first");

        
        var productToOrderItem = new List<ProductToOrderItem>();

        // loop through cart items and add them to order items and calc subtotal
        dynamic subTotal = 0;
        foreach (var cartItem in cart.CartItems)
        {
            var product = await _unitOfWork.ProductRepository.Get(cartItem.ProductId);
            if (product is null)
                return NotFound("Product not found");
            subTotal += product.Price * cartItem.Quantity;

            var productToOrder = new ProductToOrderItem
            {
                ProductId = product.Id,
                Quantity = cartItem.Quantity
            };
            productToOrderItem.Add(productToOrder);
        }
        
        var orderItem = new OrderItem
        {
            Products = productToOrderItem, 
            UserId = userId
        };


        await _unitOfWork.OrderItemRepository.Add(orderItem);
            var check = await _unitOfWork.Save();
            if (check is 0)
                return BadRequest("Error in saving order item please try again");

        // var subTotal = orderItems.Sum(x => x.Quantity * x.Product.Price);
        
        var status = dto.PaymentMethod == PaymentMethod.Cash ? OrderStatus.Placed : OrderStatus.Pending;
        
        
        var deliveryMethod = await _unitOfWork.DeliveryMethodRepository.Get(dto.DeliveryMethodId);
        if (deliveryMethod is null)
            return NotFound("Delivery method not found");
        
        
        // Create order object
        //   "paymentMethod": 0 ==> cash and status = placed
        //   "paymentMethod": 1 ==> card and status = pending
        //   "deliveryMethod": 1, standard
        //   "deliveryMethod": 2, express
        
        var paymentIntent = dto.PaymentMethod == PaymentMethod.Card ? await _paymentService.CreatePaymentIntent(Convert.ToInt64(subTotal + deliveryMethod.Price)) : null;

        var order = new Order
        {
            // Id = 15,
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            OrderItemId = orderItem.Id,
            Status = status,
            DeliveryMethodId = deliveryMethod.Id,
            PaymentMethod = dto.PaymentMethod,
            Subtotal = subTotal,
            PaymentIntentId = paymentIntent
        };

        // Add order to database
        await _unitOfWork.OrderRepository.Add(order);
        var flag = await _unitOfWork.Save();
        if (flag is not 1)
            return BadRequest("Error in saving order"); 
        await _cartRepository.ClearCart(cart,user.Id); 
        return Created("Order has been added", order);
        
            

    }





}