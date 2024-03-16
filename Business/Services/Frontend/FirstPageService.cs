using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.FirstPage;
using Taipla.Webservice.Models.Responses.Frontend;
using Taipla.Webservice.Models.Responses.Frontend.FirstPage;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class FirstPageService : IFirstPageService
    {
        private readonly TAIPLA_DbContext _dbContext;

        private readonly IHttpContextAccessor _context;

        private readonly IWebHostEnvironment _env;

        private readonly ILogger _logger;

        private readonly MessageFactory _message;

        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:FirstPageController";

        public FirstPageService(TAIPLA_DbContext dbContext
            , IWebHostEnvironment env
            , IHttpContextAccessor context
            , ILoggerFactory logger
            , MessageFactory messageFactory)
        {
            _dbContext = dbContext;
            _context = context;
            _env = env;
            _logger = logger.CreateLogger("Service");
            _message = messageFactory;
        }

        public async Task<BaseResponse> Foods(FirstPageFoodParameter param)
        {
            List<FoodsResponse> countries = new List<FoodsResponse>();

            try
            {
                var foodCountry = _dbContext.FoodCountry
                    .OrderBy(o => o.CreatedDate)
                    .Select(s => new
                    {
                        COUNTRY_ID = s.CountryId,
                        COUNTRY_NAME = s.NameTh
                    }).ToList();

                if (foodCountry == null || foodCountry != null && foodCountry.Count < 1)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Foods:204_FOOD_COUNTRY_NO_CONTENT");

                    return this.response;
                }

                var countryIds = foodCountry.Select(s => s.COUNTRY_ID).ToList();

                countryIds.ForEach(COUNTRY_ID =>
                {
                    List<FoodCenter> foods = new List<FoodCenter>();

                    var foodCenter = _dbContext.FoodCenter.Where(w => w.CountryId == COUNTRY_ID).AsQueryable();

                    if (param.limit < 1)
                    {
                        foods = foodCenter.ToList();
                    }
                    else
                    {
                        foods = foodCenter
                            .Take(param.limit)
                            .ToList();
                    }

                    var country = foodCountry.FirstOrDefault(f => f.COUNTRY_ID == COUNTRY_ID);

                    if (foods != null && foods.Count > 0)
                    {

                        var foodResponse = new FoodsResponse
                        {
                            COUNTRY_NAME = country.COUNTRY_NAME,
                            FOODS = (from food in foods
                                     let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                     select new FoodResponse
                                     {
                                         FOOD_ID_S1 = food.FoodId,
                                         FOOD_ID_S2 = food.FoodId,
                                         COUNTRY_ID = food.CountryId,
                                         CULTURE_ID = food.CultureId,
                                         NAME = food.NameTh.Trim(),
                                         IMAGE = image.image,
                                         IMAGE_SM = image.imageSM,
                                         IMAGE_MD = image.imageMD,
                                         IMAGE_LG = image.imageLG,
                                         VIEWER = food.Viewer
                                     }).ToList()
                        };

                        foodResponse.TOTAL = foodResponse.FOODS.Count;
                        countries.Add(foodResponse);
                    }
                    else
                    {
                        countries.Add(new FoodsResponse
                        {
                            TOTAL = 0,
                            COUNTRY_NAME = country.COUNTRY_NAME,
                            FOODS = new List<FoodResponse>()
                        });
                    }
                });

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Foods:200_SUCCESS");
                this.response.total = countries.Count;
                this.response.data = countries;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.Foods.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Foods:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> FoodTopViewer(FirstPageFoodTopViewerParameter param)
        {
            try
            {
                List<FoodCenter> foods = new List<FoodCenter>();

                var foodCenter = _dbContext.FoodCenter.AsQueryable()
                    .OrderByDescending(o => o.Viewer);

                if (param.limit < 1)
                {
                    foods = foodCenter.ToList();
                }
                else
                {
                    foods = foodCenter
                        .Take(param.limit)
                        .ToList();
                }

                var foodTopViewerResponse = new FoodTopViewerResponse
                {
                    TITLE = _message.GetMessage(ROOT_MESSAGE, "FoodTopViewer:TITLE_TEXT"),
                };

                if (foods != null && foods.Count > 0)
                {

                    foodTopViewerResponse.FOODS = (from food in foods
                                                   let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                                   select new FoodResponse
                                                   {
                                                       FOOD_ID_S1 = food.FoodId,
                                                       FOOD_ID_S2 = food.FoodId,
                                                       COUNTRY_ID = food.CountryId,
                                                       CULTURE_ID = food.CultureId,
                                                       NAME = food.NameTh.Trim(),
                                                       IMAGE = image.image,
                                                       IMAGE_SM = image.imageSM,
                                                       IMAGE_MD = image.imageMD,
                                                       IMAGE_LG = image.imageLG,
                                                       VIEWER = food.Viewer
                                                   }).ToList();

                    if (param.rand)
                    {
                        foodTopViewerResponse.FOODS = foodTopViewerResponse.FOODS.OrderBy(o => Guid.NewGuid()).ToList();
                    }

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "FoodTopViewer:200_SUCCESS");
                    this.response.data = foodTopViewerResponse;
                }
                else
                {
                    foodTopViewerResponse.FOODS = new List<FoodResponse>();
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "FoodTopViewer:204_FOOD_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = foodTopViewerResponse.FOODS.Count;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.FoodTopViewer.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "FoodTopViewer:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Categories(FirstPageFoodCategoriesParameter param)
        {
            try
            {
                if (param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Categories:409_COUNTRY_INVALID");

                    return this.response;
                }

                var foodCountry = _dbContext.FoodCountry.Where(w => w.CountryId == param.COUNTRY_ID).FirstOrDefault();

                if (foodCountry == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Categories:404_COUNTRY_NOT_FOUND");

                    return this.response;
                }

                CategoriesResponse categories = new CategoriesResponse()
                {
                    COUNTRY = foodCountry.NameTh?.Trim() ?? string.Empty,
                    FOODS = new List<CategoriesCulturesResponse>()
                };

                List<FoodCulture> foodCultues = new List<FoodCulture>();
                var foodCulture = _dbContext.FoodCulture.Where(w => w.CountryId == param.COUNTRY_ID).AsQueryable();

                if (param.CULTURE_ID > 0)
                {
                    foodCultues = foodCulture.Where(w => w.CultureId == param.CULTURE_ID).ToList();
                }
                else
                {
                    foodCultues = foodCulture.ToList();
                }

                if (foodCultues == null || foodCultues.Count < 1)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Categories:204_CULTURE_NO_CONTENT");

                    return this.response;
                }

                List<CategoriesCulturesResponse> cultures = new List<CategoriesCulturesResponse>();

                foodCultues.ForEach(CULTURE =>
                {
                    CategoriesCulturesResponse category = new CategoriesCulturesResponse()
                    {
                        CULTURE = CULTURE.NameTh?.Trim() ?? string.Empty,
                        FOOD_CULTURE = new List<FoodResponse>()
                    };

                    List<FoodCenter> foods = new List<FoodCenter>();
                    var foodCenter = _dbContext.FoodCenter.Where(w => w.CountryId == CULTURE.CountryId && w.CultureId == CULTURE.CultureId).AsQueryable();

                    if (param.limit < 1)
                    {
                        foods = foodCenter.ToList();
                    }
                    else
                    {
                        foods = foodCenter.
                            Take(param.limit)
                            .ToList();
                    }

                    if (foods != null && foods.Count > 0)
                    {
                        category.FOOD_CULTURE = (from food in foods
                                                 let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                                 select new FoodResponse
                                                 {
                                                     FOOD_ID_S1 = food.FoodId,
                                                     FOOD_ID_S2 = food.FoodId,
                                                     COUNTRY_ID = food.CountryId,
                                                     CULTURE_ID = food.CultureId,
                                                     NAME = food.NameTh.Trim(),
                                                     IMAGE = image.image,
                                                     IMAGE_SM = image.imageSM,
                                                     IMAGE_MD = image.imageMD,
                                                     IMAGE_LG = image.imageLG,
                                                     VIEWER = food.Viewer
                                                 }).ToList();

                        category.TOTAL = category.FOOD_CULTURE.Count;
                    }

                    cultures.Add(category);
                });

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Categories:200_SUCCESS");
                this.response.total = cultures.Count;
                this.response.data = cultures;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.Categories.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Categories:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Cultures(FirstPageFoodCulturesParameter param)
        {
            try
            {
                if (param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:409_COUNTRY_INVALID");

                    return this.response;
                }

                var foodCountry = _dbContext.FoodCountry.Where(w => w.CountryId == param.COUNTRY_ID).FirstOrDefault();

                if (foodCountry == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:404_COUNTRY_NOT_FOUND");

                    return this.response;
                }

                if (param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:409_CULTURE_INVALID");

                    return this.response;
                }

                var foodCulture = _dbContext.FoodCulture.Where(w => w.CountryId == param.COUNTRY_ID && w.CultureId == param.CULTURE_ID).FirstOrDefault();

                if (foodCulture == null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:204_CULTURE_NO_CONTENT");

                    return this.response;
                }

                CulturesResponse culture = new CulturesResponse()
                {
                    COUNTRY = foodCountry.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    CULTURE = foodCulture.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    FOODS = new List<FoodResponse>()
                };

                List<FoodCenter> foods = new List<FoodCenter>();
                var foodCenter = _dbContext.FoodCenter.Where(w => w.CountryId == foodCulture.CountryId && w.CultureId == foodCulture.CultureId).AsQueryable();

                if (param.limit < 1)
                {
                    foods = foodCenter.ToList();
                }
                else
                {
                    foods = foodCenter.
                        Take(param.limit)
                        .ToList();
                }

                if (foods != null && foods.Count > 0)
                {
                    culture.FOODS = (from food in foods
                                     let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                     select new FoodResponse
                                     {
                                         FOOD_ID_S1 = food.FoodId,
                                         FOOD_ID_S2 = food.FoodId,
                                         COUNTRY_ID = food.CountryId,
                                         CULTURE_ID = food.CultureId,
                                         NAME = food.NameTh.Trim(),
                                         IMAGE = image.image,
                                         IMAGE_SM = image.imageSM,
                                         IMAGE_MD = image.imageMD,
                                         IMAGE_LG = image.imageLG,
                                         VIEWER = food.Viewer
                                     }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:204_FOOD_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = culture.FOODS.Count;
                this.response.data = culture;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.Cultures.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Cultures:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> RestaurantTopViewer(FirstPageRestaurantParameter param)
        {
            RestaurantTopViewerResponse response = new RestaurantTopViewerResponse();
            response.TITLE = "รายการอาหารยอดนิยม";
            try
            {
                var restaurants = _dbContext.Restaurant
                    .OrderByDescending(o => o.Viewer)
                    .Take(param.limit)
                    .ToList();

                if (restaurants != null && restaurants.Count > 0)
                {
                    response.RESTAURANTS = (from res in restaurants
                                            let image = _env.GetImageThumbnail(_context, res?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                            select new RestaurantResponse
                                            {
                                                RES_ID_S1 = res.ResId,
                                                RES_ID_S2 = res.ResId,
                                                COUNTRY_ID = res.CountryId,
                                                NAME = res.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                                VIEWER = res.Viewer,
                                                IMAGE = (image != null) ? image.image : string.Empty,
                                                IMAGE_SM = (image != null) ? image.imageSM : string.Empty,
                                                IMAGE_MD = (image != null) ? image.imageMD : string.Empty,
                                                IMAGE_LG = (image != null) ? image.imageLG : string.Empty
                                            }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantTopViewer:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantTopViewer:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = response.RESTAURANTS.Count;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.RestaurantTopViewer.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantTopViewer:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> RestaurantPromotions(FirstPageRestaurantParameter param)
        {
            List<PromotionResponse> response = new List<PromotionResponse>();

            try
            {
                var now = DateTime.Now;

                var promotions = _dbContext.Promotion
                    .Where(w => w.Flag != (int)PromotionEnum.HIDE)
                    .Where(w => (w.Flag == (int)PromotionEnum.FREEZE || (w.Flag == (int)PromotionEnum.USE_ESTIMATE &&
                        w.StartDate != null &&
                        w.EndDate != null &&
                        w.StartDate <= now && w.EndDate >= now)))
                    .OrderByDescending(o => o.Viewer)
                    .Take(param.limit)
                    .ToList();

                if (promotions != null && promotions.Count > 0)
                {
                    response = (from pro in promotions
                                let image = _env.GetImageThumbnail(_context, pro.Thumbnail, ImageExtension.DEFAULT_IMAGE)
                                select new PromotionResponse
                                {
                                    PROMOTION_ID = pro.Id,
                                    RESTAURANT_ID = pro.ResId,
                                    NAME = pro.Name.Trim()?.DecodeSpacialCharacters() ?? string.Empty,
                                    VIEWER = pro.Viewer,
                                    IMAGE = (image != null) ? image.image : string.Empty,
                                    IMAGE_SM = (image != null) ? image.imageSM : string.Empty,
                                    IMAGE_MD = (image != null) ? image.imageMD : string.Empty,
                                    IMAGE_LG = (image != null) ? image.imageLG : string.Empty
                                }).ToList();
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotions:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotions:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = response.Count;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.RestaurantPromotions.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantPromotions:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> RestaurantNear(FirstPageRestaurantNearParameter param)
        {
            try
            {
                RestaurantNearResponse response = new RestaurantNearResponse();
                response.TITLE = "ร้านอาหารใกล้คุณ";

                var restaurants = _dbContext.Restaurant
                        .Take(param.limit)
                        .ToList();

                if (restaurants != null && restaurants.Count > 0)
                {
                    var rand = new Random();

                    response.RESTAURANTS = (from res in restaurants
                                            let lat = (res.Latitude == null ? null : (double?)res.Latitude)
                                            let lon = (res.Longitude == null ? null : (double?)res.Latitude)
                                            let image = _env.GetImageThumbnail(_context, res?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                            select new NearResponse
                                            {
                                                RES_ID_S1 = res.ResId,
                                                RES_ID_S2 = res.ResId,
                                                COUNTRY_ID = res.CountryId,
                                                NAME = res.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                                VIEWER = res.Viewer,
                                                IMAGE = (image != null) ? image.image : string.Empty,
                                                IMAGE_SM = (image != null) ? image.imageSM : string.Empty,
                                                IMAGE_MD = (image != null) ? image.imageMD : string.Empty,
                                                IMAGE_LG = (image != null) ? image.imageLG : string.Empty,
                                                coordinates = lat != null && lon != null ? new List<double>
                                                {
                                                    lat.Value,
                                                    lon.Value
                                                } : null,
                                                distances = rand.Next(1, 30)
                                            })
                                            .OrderBy(o => o.distances)
                                            .ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantNear:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantNear:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = response.RESTAURANTS.Count;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FirstPageService.RestaurantNear.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "RestaurantNear:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }
    }
}
