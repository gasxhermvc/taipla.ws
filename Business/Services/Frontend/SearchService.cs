using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Search;
using Taipla.Webservice.Models.Responses.Frontend;
using Taipla.Webservice.Models.Responses.Frontend.Search;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class SearchService : ISearchService
    {
        private readonly TAIPLA_DbContext _dbContext;

        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _context;

        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;

        private readonly MessageFactory _message;

        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:SearchController";

        public SearchService(TAIPLA_DbContext dbContext
            , IConfiguration configuration
            , IHttpContextAccessor context
            , ILoggerFactory logger
            , IWebHostEnvironment env
            , MessageFactory messageFactory)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
            _message = messageFactory;
        }

        public async Task<BaseResponse> Histories(SearchHistoryParameter param)
        {
            List<object> histories = new List<object>();
            try
            {

                var userDevice = _dbContext.UserUtilities.GetDevice(param.DEVICE_ID);

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Histories:404_DEVICE_NOT_FOUND");

                    return this.response;
                }

                List<HistorySearch> historySearchs = null;

                var queryHistorySearch = (from hs in _dbContext.HistorySearch
                                          where hs.Deleted == (int)DeletedEnum.Delete && hs.DeviceId == param.DEVICE_ID
                                          orderby hs.CreatedDate descending
                                          select hs).AsQueryable();

                if (param.limit < 1)
                {
                    historySearchs = queryHistorySearch.ToList();
                }
                else
                {
                    historySearchs = queryHistorySearch
                        .Take(param.limit)
                        .ToList();
                }

                if (historySearchs != null && historySearchs.Count > 0)
                {
                    histories = (from o in historySearchs
                                 let text = o.SearchText
                                 let condition = JsonConvert.SerializeObject(o.Condition)
                                 let concatText = string.Format("{0}.{1}", text, condition)
                                 let hash = HashExtension.Sha256(concatText)
                                 select new HistoriesResponse
                                 {
                                     Id = o.Id,
                                     SearchText = o.SearchText,
                                     Condition = JsonConvert.DeserializeObject<SearchConditionParameter>(o.Condition.DecodeSpacialCharacters()),
                                     Hashing = hash,
                                     CreatedDate = o.CreatedDate
                                 } as object)
                                 .ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Histories:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Histories:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = histories.Count;
                this.response.data = new
                {
                    title = _message.GetMessage(ROOT_MESSAGE, "Histories:TITLE_TEXT"),
                    histories = histories
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.SearchService.Histories.Exception : {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Histories:500_INTERNAL_SERVER_ERROR");
                this.response.data = histories;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Search(SearchParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserUtilities.GetDevice(param.DEVICE_ID);

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    //=>ลบประวัติไม่สำเร็จ, เนื่องจากไม่พบรหัสอุปกรณ์
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:404_DEVICE_NOT_FOUND");

                    return this.response;
                }

                if (string.IsNullOrEmpty(param.TEXT) && param.condition == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:409_TEXT_IS_EMPTY_AND_CONDITION_IS_NULL");

                    return this.response;
                }

                //=>ตรวจสอบข้อมูล condition
                if (param.condition != null && !ValidateExtension.isDirectionValid((int)param.condition.DIRECTION))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:409_DIRECTION_IN_VALID");

                    return this.response;
                }

                if (param.condition != null && param.condition.COUNTRY_ID > 0)
                {
                    var foodCountry = _dbContext.FoodCountry.Where(w => w.CountryId == param.condition.COUNTRY_ID).FirstOrDefault();

                    if (foodCountry == null)
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.NotFound;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:404_COUNTRY_NOT_FOUND");

                        return this.response;
                    }
                }

                if (param.condition != null && param.condition.CULTURE_ID > 0)
                {
                    var foodCulture = _dbContext.FoodCulture.Where(w => w.CultureId == param.condition.CULTURE_ID).FirstOrDefault();

                    if (foodCulture == null)
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.NotFound;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:404_CULTURE_NOT_FOUND");

                        return this.response;
                    }
                }

                DateTime now = DateTime.Now;

                HistorySearch historySearch = new HistorySearch
                {
                    ClientId = param.DEVICE_ID,
                    DeviceId = param.DEVICE_ID,
                    SearchText = param.TEXT,
                    Condition = param.condition != null ? JsonConvert.SerializeObject(param.condition, Formatting.Indented).EncodeSpacialCharacters() : string.Empty,
                    Deleted = 0,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.HistorySearch.Add(historySearch);
                    _dbContext.SaveChanges();
                });

                if (!result.success)
                {
                    _logger.LogError("Frontend.SearchService.Search.Transaction.Exception : {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:400_BAD_REQUEST");

                    return this.response;
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:200_SUCCESS");
                this.response.data = null;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.SearchService.Search.Exception : {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Search:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> HistoryRemove(SearchHistoryRemoveParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserUtilities.GetDevice(param.DEVICE_ID);

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:404_DEVICE_NOT_FOUND");

                    return this.response;
                }

                var historySearch = _dbContext.HistorySearch.Where(w => w.Id == param.SEARCH_ID && w.DeviceId == param.DEVICE_ID).FirstOrDefault();

                if (historySearch == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:404_HISTORY_SEARCH_NOT_FOUND");

                    return this.response;
                }

                if (historySearch.Deleted == (int)DeletedEnum.Deleted)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:403_HISTORY_SEARCH_IS_DELETED");

                    return this.response;
                }

                var now = DateTime.Now;
                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    historySearch.Deleted = (int)DeletedEnum.Deleted;
                    historySearch.UpdatedDate = now;
                    _dbContext.HistorySearch.Update(historySearch);
                    _dbContext.SaveChanges();
                });

                if (!result.success)
                {
                    _logger.LogError("Frontend.SearchService.HistoryRemove.Transaction.Exception : {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:400_BAD_REQUEST");

                    return this.response;
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:200_SUCCESS");
                this.response.data = null;

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.SearchService.HistoryRemove.Exception : {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemove:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> HistoryRemoveAll(SearchHistoryRemoveAllParameter param)
        {
            try
            {
                var userDevice = _dbContext.UserUtilities.GetDevice(param.DEVICE_ID);

                if (userDevice == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:404_DEVICE_NOT_FOUND");

                    return this.response;
                }

                var historySearchs = _dbContext.HistorySearch.Where(w => w.DeviceId == param.DEVICE_ID && w.Deleted == (int)DeletedEnum.Delete).ToList();

                if (historySearchs == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:404_HISTORY_SEARCH_NOT_FOUND");

                    return this.response;
                }

                if (historySearchs != null && historySearchs.Count < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:404_HISTORY_SEARCH_NOT_FOUND");

                    return this.response;
                }

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    historySearchs.ForEach(historySearch =>
                    {
                        historySearch.Deleted = (int)DeletedEnum.Deleted;
                        historySearch.UpdatedDate = DateTime.Now;
                    });
                    _dbContext.HistorySearch.UpdateRange(historySearchs);
                    _dbContext.SaveChanges();
                });

                if (!result.success)
                {
                    _logger.LogError("Frontend.SearchService.HistoryRemove.Transaction.Exception : {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:400_BAD_REQUEST");

                    return this.response;
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:200_SUCCESS");
                this.response.total = historySearchs.Count;
                this.response.data = null;

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.SearchService.HistoryRemove.Exception : {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "HistoryRemoveAll:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Results(SearchParameter param)
        {
            SearchResultResponse response = new SearchResultResponse();
            response.TITLE = "ผลลัพธ์การค้นหา";
            try
            {
                if (string.IsNullOrEmpty(param.TEXT) &&
                    param.condition.COUNTRY_ID < 1 &&
                    param.condition.CULTURE_ID < 1 &&
                    param.condition.TAGS.Count < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:400_PARAMETER_INVALID");

                    return this.response;
                }

                List<int> _foods = new List<int>();
                List<int> _restaurants = new List<int>();
                List<int> _resIds = new List<int>();

                List<FoodCenter> foods = new List<FoodCenter>();
                List<Restaurant> restaurants = new List<Restaurant>();

                if (!string.IsNullOrEmpty(param.TEXT))
                {
                    var text = param.TEXT.EncodeSpacialCharacters();

                    var foodQueryText = _dbContext.FoodCenter.Where(w => w.NameTh.StartsWith(text) ||
                        w.NameTh.Contains(text) ||
                        w.NameTh.EndsWith(text) ||
                        w.Description.StartsWith(text) ||
                        w.Description.Contains(text) ||
                        w.Description.EndsWith(text))
                        .AsQueryable();

                    if (param.condition.COUNTRY_ID > 0)
                    {
                        foodQueryText = foodQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                    }

                    if (param.condition.CULTURE_ID > 0)
                    {
                        foodQueryText = foodQueryText.Where(w => w.CultureId == param.condition.CULTURE_ID);
                    }

                    var foodText = foodQueryText
                        .Select(s => s.FoodId)
                        .ToList();

                    if (foodText != null && foodText.Count > 0)
                    {
                        _foods = _foods.Union(foodText).ToList();
                    }

                    var resQueryText = _dbContext.Restaurant.Where(w => w.Name.StartsWith(text) ||
                      w.Name.Contains(text) ||
                      w.Name.EndsWith(text))
                        .AsQueryable();

                    if (param.condition.COUNTRY_ID > 0)
                    {
                        resQueryText = resQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                    }

                    var resText = resQueryText.Select(s => s.ResId)
                        .ToList();

                    if (resText != null && resText.Count > 0)
                    {
                        _restaurants = _restaurants.Union(resText).ToList();
                    }

                    var resMenuQueryText = _dbContext.RestaurantMenu.Where(w => w.NameTh.StartsWith(text) ||
                        w.NameTh.Contains(text) ||
                        w.NameTh.EndsWith(text) ||
                        w.Description.StartsWith(text) ||
                        w.Description.Contains(text) ||
                        w.Description.EndsWith(text))
                        .AsQueryable();

                    if (param.condition.COUNTRY_ID > 0)
                    {
                        resMenuQueryText = resMenuQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                    }

                    if (param.condition.CULTURE_ID > 0)
                    {
                        resMenuQueryText = resMenuQueryText.Where(w => w.CultureId == param.condition.CULTURE_ID);
                    }

                    var resMenuText = resMenuQueryText
                        .Select(s => s.ResId)
                        .ToList();

                    if (resMenuText != null && resMenuText.Count > 0)
                    {
                        _resIds = _resIds.Union(resMenuText).ToList();
                    }
                }

                if (param.condition.TAGS.Count > 0)
                {
                    param.condition.TAGS.ForEach(f =>
                    {
                        if (string.IsNullOrEmpty(f))
                        {
                            return;
                        }

                        var text = f.EncodeSpacialCharacters();

                        var foodQueryText = _dbContext.FoodCenter.Where(w => w.NameTh.StartsWith(text) ||
                           w.NameTh.Contains(text) ||
                           w.NameTh.EndsWith(text))
                            .AsQueryable();

                        if (param.condition.COUNTRY_ID > 0)
                        {
                            foodQueryText = foodQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                        }

                        if (param.condition.CULTURE_ID > 0)
                        {
                            foodQueryText = foodQueryText.Where(w => w.CultureId == param.condition.CULTURE_ID);
                        }

                        var foodText = foodQueryText
                            .Select(s => s.FoodId)
                            .ToList();

                        if (foodText != null && foodText.Count > 0)
                        {
                            _foods = _foods.Union(foodText).ToList();
                        }

                        var resQueryText = _dbContext.Restaurant.Where(w => w.Name.StartsWith(text) ||
                              w.Name.Contains(text) ||
                              w.Name.EndsWith(text) ||
                              w.Tags.StartsWith(text) ||
                              w.Tags.Contains(text) ||
                              w.Tags.EndsWith(text))
                            .AsQueryable();

                        if (param.condition.COUNTRY_ID > 0)
                        {
                            resQueryText = resQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                        }

                        var resText = resQueryText.Select(s => s.ResId)
                            .ToList();

                        if (resText != null && resText.Count > 0)
                        {
                            _restaurants = _restaurants.Union(resText).ToList();
                        }

                        var resMenuQueryText = _dbContext.RestaurantMenu.Where(w => w.NameTh.StartsWith(text) ||
                            w.NameTh.Contains(text) ||
                            w.NameTh.EndsWith(text) ||
                            w.Description.StartsWith(text) ||
                            w.Description.Contains(text) ||
                            w.Description.EndsWith(text))
                            .AsQueryable();

                        if (param.condition.COUNTRY_ID > 0)
                        {
                            resMenuQueryText = resMenuQueryText.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                        }

                        if (param.condition.CULTURE_ID > 0)
                        {
                            resMenuQueryText = resMenuQueryText.Where(w => w.CultureId == param.condition.CULTURE_ID);
                        }

                        var resMenuText = resMenuQueryText
                            .Select(s => s.ResId)
                            .ToList();

                        if (resMenuText != null && resMenuText.Count > 0)
                        {
                            _resIds = _resIds.Union(resMenuText).ToList();
                        }
                    });
                }

                if (_foods != null && _foods.Count > 0)
                {
                    _foods = _foods.Distinct().ToList();
                    var foodQuery = _dbContext.FoodCenter.Where(w => _foods.Contains(w.FoodId)).AsQueryable();

                    if (param.condition.COUNTRY_ID > 0)
                    {
                        foodQuery = foodQuery.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                    }

                    if (param.condition.CULTURE_ID > 0)
                    {
                        foodQuery = foodQuery.Where(w => w.CultureId == param.condition.CULTURE_ID);
                    }

                    foods = foodQuery.ToList();
                }

                if (_resIds != null && _resIds.Count > 0)
                {
                    //=>Union ID From Res menu
                    _restaurants = _restaurants.Union(_resIds).ToList();
                }

                if (_restaurants != null && _restaurants.Count > 0)
                {
                    _restaurants = _restaurants.Distinct().ToList();
                    var resQuery = _dbContext.Restaurant.Where(w => _restaurants.Contains(w.ResId)).AsQueryable();

                    if (param.condition.COUNTRY_ID > 0)
                    {
                        resQuery = resQuery.Where(w => w.CountryId == param.condition.COUNTRY_ID);
                    }

                    restaurants = resQuery.ToList();
                }

                List<FoodResponse> foodResults = null;
                List<RestaurantResponse> restaurantResult = null;

                if (foods != null && foods.Count > 0)
                {
                    foodResults = (from food in foods
                                   let image = _env.GetImageThumbnail(_context, food.Thumbnail, ImageExtension.DEFAULT_IMAGE)
                                   select new FoodResponse
                                   {
                                       FOOD_ID_S1 = food.FoodId,
                                       FOOD_ID_S2 = food.FoodId,
                                       COUNTRY_ID = food.CountryId,
                                       CULTURE_ID = food.CultureId,
                                       NAME = food.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                       VIEWER = food.Viewer,
                                       IMAGE = (image != null) ? image.image : string.Empty,
                                       IMAGE_SM = (image != null) ? image.imageSM : string.Empty,
                                       IMAGE_MD = (image != null) ? image.imageMD : string.Empty,
                                       IMAGE_LG = (image != null) ? image.imageLG : string.Empty
                                   }).ToList();
                }

                if (restaurants != null && restaurants.Count > 0)
                {
                    restaurantResult = (from res in restaurants
                                        let image = _env.GetImageThumbnail(_context, res.Thumbnail, ImageExtension.DEFAULT_IMAGE)
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
                }

                switch (param.condition.DIRECTION)
                {
                    case DirectionEnum.FOOD_CENTER:
                        response.foodResult = foodResults ?? new List<FoodResponse>();
                        if (response.foodResult.Count > 0)
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:200_SUCCESS");
                            this.response.statusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:204_NO_CONTENT");
                            this.response.statusCode = (int)HttpStatusCode.NoContent;
                        }
                        break;
                    case DirectionEnum.RESTAURANT:
                        response.restaurantResult = restaurantResult ?? new List<RestaurantResponse>();
                        if (response.restaurantResult.Count > 0)
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:200_SUCCESS");
                            this.response.statusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:204_NO_CONTENT");
                            this.response.statusCode = (int)HttpStatusCode.NoContent;
                        }
                        break;
                    case DirectionEnum.ALL:
                    case DirectionEnum.ALL_:
                        response.foodResult = foodResults ?? new List<FoodResponse>();
                        response.restaurantResult = restaurantResult ?? new List<RestaurantResponse>();
                        if ((response.foodResult.Count > 0 ||
                            response.restaurantResult.Count > 0))
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:200_SUCCESS");
                            this.response.statusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:204_NO_CONTENT");
                            this.response.statusCode = (int)HttpStatusCode.NoContent;
                        }
                        break;
                }

                this.response.success = true;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.SearchService.Results.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Results:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
    }
}
