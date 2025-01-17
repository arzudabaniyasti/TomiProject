﻿using AutoMapper;
using MediatR;
using Tomi.Domain.Enums.CouponEnums;
using Tomi.Application.Models;
using Tomi.Domain.Entities;
using Tomi.Domain.IRepositories;

namespace Tomi.Application.Features.Coupons.Commands.Apply
{
    public class ApplyCouponHandler : IRequestHandler<ApplyCouponCommand, ApplyCouponModel>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ApplyCouponHandler(ICouponRepository couponRepository, IShoppingCartRepository shoppingCartRepository, IMapper mapper, IProductRepository productRepository)
        {
            _couponRepository = couponRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<ApplyCouponModel> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _couponRepository.GetByCouponIdAsync(request.CouponId);
            if (coupon == null)
            {
                throw new InvalidOperationException("Coupon not found.");
            }

            var shoppingCart = await _shoppingCartRepository.GetByUserIdAsync(request.UserId);
            if (shoppingCart == null)
            {
                throw new InvalidOperationException("Shopping cart not found.");
            }

            decimal basketTotalPrice = await CalculateBasketTotalPrice(shoppingCart.ShoppingCartItemList);
            decimal basketFinalValue = ApplyDiscount(basketTotalPrice, coupon);

            shoppingCart.CouponId = request.CouponId;
            await _shoppingCartRepository.UpdateAsync(shoppingCart.Id, shoppingCart);

            return new ApplyCouponModel
            {
                BasketTotalPrice = basketTotalPrice,
                CouponId = coupon.CouponId,
                BasketFinalValue = basketFinalValue
            };

            async Task<decimal> CalculateBasketTotalPrice(List<ShoppingCartItem> itemList)
            {
                decimal totalPrice = 0;
                foreach (var item in itemList)
                {
                    var product = await _productRepository.GetByIdAsync(item.ItemId);
                    totalPrice += product.Price * item.Count;
                }
                return totalPrice;
            }

            decimal ApplyDiscount(decimal totalPrice, Coupon coupon)
            {
                var discountType = (DiscountType)Enum.Parse(typeof(DiscountType), coupon.DiscountType);
                return discountType switch
                {
                    DiscountType.Amount => totalPrice - coupon.Value,
                    DiscountType.Ratio => totalPrice * (1 - coupon.Value / 100m),
                    _ => throw new InvalidOperationException("Invalid discount type."),
                };
            }
        }
    }
}
