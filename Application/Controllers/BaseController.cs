using System.Security.Claims;
using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace kursah_5semestr.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        private IProductsService _productsService;
        private ICartItemsService _cartItemsService;
        private IUsersService _usersService;

        public BaseController(IProductsService productsService, ICartItemsService cartItemsService, IUsersService usersService)
        {
            _productsService = productsService;
            _cartItemsService = cartItemsService;
            _usersService = usersService;
        }

        [Route("/products")]
        [HttpPost]
        public async Task<ActionResult<ProductOutDto>> PostProduct(ProductDto dto)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null || user.Login != "admin")
            {
                return Unauthorized(new StatusOutDto("error", "Only admin can manage products"));
            }
            var (product, error) = Product.Create(dto.Title, dto.Price, dto.Quantity, dto.Description);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new StatusOutDto("error", error));
            }
            var saved = await _productsService.CreateProduct(product);
            var outDto = new ProductOutDto(product.Id, product.Title, product.Price, product.Quantity, product.Description);
            return CreatedAtAction(nameof(PostProduct), new { id = product.Id }, outDto);
        }

        [Route("/products/{id}")]
        [HttpPatch]
        public async Task<ActionResult<ProductOutDto>> PatchProduct(Guid id, ProductDto dto)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null || user.Login != "admin")
            {
                return Unauthorized(new StatusOutDto("error", "Only admin can manage products"));
            }
            var patch = new Product();
            patch.Title = dto.Title;
            patch.Description = dto.Description;
            patch.Price = dto.Price;
            patch.Quantity = dto.Quantity;
            var saved = await _productsService.UpdateProduct(id, patch);
            if (saved != null)
            { 
                var outDto = new ProductOutDto(saved.Id, saved.Title, saved.Price, saved.Quantity, saved.Description);
                return Ok(outDto);
            } 
            else
            {
                return NotFound(new StatusOutDto("error"));
            }
        }

        [Route("/products")]
        [HttpGet]
        public ActionResult<IList<ProductOutDto>> GetProducts()
        {
            var products = _productsService.GetProducts();
            var outDtos = products.Select(p => new ProductOutDto(p.Id, p.Title, p.Price, p.Quantity, p.Description));
            return Ok(outDtos);
        }

        [Route("/products/{id}")]
        [HttpDelete]
        public async Task<ActionResult<ProductOutDto>> DeleteProduct(Guid id)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null || user.Login != "admin")
            {
                return Unauthorized(new StatusOutDto("error", "Only admin can manage products"));
            }
            var ok = await _productsService.DeleteProduct(id);
            if (ok)
            {
                return Ok(new StatusOutDto("ok"));
            }
            else
            {
                return Ok(new StatusOutDto("error"));
            }
        }

        [Route("/cart_items")]
        [HttpPost]
        public async Task<ActionResult<CartItemOutDto>> PostCartItem(CartItemDto dto)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null) {
                return Unauthorized(new StatusOutDto("error"));
            }
            if (dto.ProductId == null)
            {
                return BadRequest(new StatusOutDto("error", "Product ID is mandatory"));
            }
            var product = _productsService.GetProductById((Guid)dto.ProductId!);
            if (product == null)
            {
                return NotFound(new StatusOutDto("error", "Product not found"));
            }
            var (cartItem, error) = CartItem.Create(product, user, dto.Quantity);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new StatusOutDto("error", error));
            }
            var saved = await _cartItemsService.CreateCartItem(cartItem);
            var outDto = new CartItemOutDto(cartItem.Id, cartItem.Product, cartItem.Quantity);
            return CreatedAtAction(nameof(PostCartItem), new { id = cartItem.Id }, outDto);
        }

        [Route("/cart_items/{id}")]
        [HttpPatch]
        public async Task<ActionResult<CartItemOutDto>> PatchCartItem(Guid id, CartItemDto dto)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null)
            {
                return Unauthorized(new StatusOutDto("error"));
            }
            var patch = new CartItem();
            patch.Quantity = dto.Quantity;
            var saved = await _cartItemsService.UpdateCartItem(id, patch);
            if (saved != null)
            {
                var outDto = new CartItemOutDto(saved.Id, saved.Product, saved.Quantity);
                return Ok(outDto);
            }
            else
            {
                return NotFound(new StatusOutDto("error"));
            }
        }

        [Route("/cart_items")]
        [HttpGet]
        public ActionResult<IList<CartItemOutDto>> GetCartItems()
        {
            var CartItems = _cartItemsService.GetCartItems();
            var outDtos = CartItems.Select(ci => new CartItemOutDto(ci.Id, ci.Product, ci.Quantity));
            return Ok(outDtos);
        }

        [Route("/cart_items/{id}")]
        [HttpDelete]
        public async Task<ActionResult<CartItemOutDto>> DeleteCartItem(Guid id)
        {
            var ok = await _cartItemsService.DeleteCartItem(id);
            if (ok)
            {
                return Ok(new StatusOutDto("ok"));
            }
            else
            {
                return Ok(new StatusOutDto("error"));
            }
        }

    }
}
