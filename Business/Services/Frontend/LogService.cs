using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Log;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class LogService : ILogService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<ClientInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private readonly MessageFactory _message;
        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:LogController";

        public LogService(TAIPLA_DbContext dbContext
            , IAuthenticationService<ClientInfo> authen
            , IHttpContextAccessor context
            , ILoggerFactory logger
            , IWebHostEnvironment env
            , MessageFactory messageFactory)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
            _message = messageFactory;
        }

        public async Task<BaseResponse> Restaurant(LogRestaurantParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserDevice.Where(w => w.ClientId == param.deviceId || w.DeviceId == param.deviceId)
                    .FirstOrDefault();

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Restaurant:403_DEVICE_NOT_FOUND");

                    return this.response;
                }


                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.restaurantId).FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Restaurant:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                restaurant.Viewer += 1;
                restaurant.UpdatedDate = DateTime.Now;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Restaurant.Update(restaurant);
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Restaurant:200_SUCCESS");
                    this.response.data = new
                    {
                        resId = restaurant.ResId,
                        viewer = restaurant.Viewer
                    };
                }
                else
                {
                    _logger.LogError("Frontend.LogService.Restaurant.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Restaurant:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.LogService.Restaurant.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Restaurant:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
        public async Task<BaseResponse> RestaurantMenu(LogRestaurantMenuParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserDevice.Where(w => w.ClientId == param.deviceId || w.DeviceId == param.deviceId)
                    .FirstOrDefault();

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:403_DEVICE_NOT_FOUND");

                    return this.response;
                }


                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.restaurantId).FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }


                var restaurantMenu = _dbContext.RestaurantMenu.Where(w => w.ResId == param.restaurantId && w.MenuId == param.menuId).FirstOrDefault();

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:404_RESTAURANT_MENU_NOT_FOUND");

                    return this.response;
                }

                restaurantMenu.Viewer += 1;
                restaurantMenu.UpdatedDate = DateTime.Now;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.RestaurantMenu.Update(restaurantMenu);
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:200_SUCCESS");
                    this.response.data = new
                    {
                        resId = restaurant.ResId,
                        menuId = restaurantMenu.MenuId,
                        viewer = restaurantMenu.Viewer
                    };
                }
                else
                {
                    _logger.LogError("Frontend.LogService.RestaurantMenu.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.LogService.RestaurantMenu.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantMenu:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
        public async Task<BaseResponse> RestaurantPromotion(LogRestaurantPromotionParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserDevice.Where(w => w.ClientId == param.deviceId || w.DeviceId == param.deviceId)
                    .FirstOrDefault();

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:403_DEVICE_NOT_FOUND");

                    return this.response;
                }


                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.restaurantId).FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }


                var restaurantPromotion = _dbContext.Promotion.Where(w => w.ResId == param.restaurantId && w.Id == param.promotionId).FirstOrDefault();
                if (restaurantPromotion == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:404_RESTAURANT_PROMOTION_NOT_FOUND");
                    return this.response;
                }

                restaurantPromotion.Viewer += 1;
                restaurantPromotion.UpdatedDate = DateTime.Now;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Promotion.Update(restaurantPromotion);
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:200_SUCCESS");
                    this.response.data = new
                    {
                        resId = restaurant.ResId,
                        promotionId = restaurantPromotion.Id,
                        viewer = restaurantPromotion.Viewer
                    };
                }
                else
                {
                    _logger.LogError("Frontend.LogService.RestaurantPromotion.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.LogService.RestaurantPromotion.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotion:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
        public async Task<BaseResponse> Food(LogFoodParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserDevice.Where(w => w.ClientId == param.deviceId || w.DeviceId == param.deviceId)
                    .FirstOrDefault();

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Food:403_DEVICE_NOT_FOUND");

                    return this.response;
                }

                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId).FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Food:404_Food_NOT_FOUND");

                    return this.response;
                }

                foodCenter.Viewer += 1;
                foodCenter.UpdatedDate = DateTime.Now;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCenter.Update(foodCenter);
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Food:200_SUCCESS");
                    this.response.data = new
                    {
                        foodId = foodCenter.FoodId,
                        viewer = foodCenter.Viewer
                    };
                }
                else
                {
                    _logger.LogError("Frontend.LogService.Food.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Food:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.LogService.Food.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Food:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
    }
}
