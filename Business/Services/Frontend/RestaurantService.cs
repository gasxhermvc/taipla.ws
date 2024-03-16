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
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Restaurant;
using Taipla.Webservice.Models.Responses.Frontend;
using Taipla.Webservice.Models.Responses.Frontend.Restaurant;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class RestaurantService : IRestaurantService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<ClientInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private readonly MessageFactory _message;
        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:RestaurantController";

        public RestaurantService(TAIPLA_DbContext dbContext
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

        public async Task<BaseResponse> Detail(RestaurantParameter param)
        {
            try
            {
                RestaurantDetailResponse res = null;

                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                //=>Thumbnail
                var media = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.RESTAURANT.GetString(),
                    restaurant.ResId.ToString());


                List<Media> medias = new List<Media>();

                //=>Sum vote
                var scores = _dbContext.Vote.Where(w => w.RefId == param.RESTAURANT_ID.ToString() &&
                    w.SystemName == UploadEnum.RESTAURANT.GetString())
                    .Select(s => new
                    {
                        score = s.Score
                    }).ToList();

                double avg = Math.Round(scores.Count == 0 ? 0 : (double)scores.Sum(s => s.score) / (double)scores.Count, 2);

                res = new RestaurantDetailResponse()
                {
                    RESTAURANT_ID_S1 = restaurant.ResId,
                    RESTAURANT_ID_S2 = restaurant.ResId,
                    COUNTRY_ID = restaurant.CountryId,
                    NAME = restaurant.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    VIEWER = restaurant.Viewer,
                    RATING = avg,
                    ADDRESS = restaurant.Address?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    PHONE = restaurant.Phone?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    COORDINATES = (restaurant.Latitude != null && restaurant.Longitude != null) ? new List<double>()
                    {
                        (double)restaurant.Latitude,
                        (double)restaurant.Longitude
                    } : null,
                    WEBSITE = restaurant.Website?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    VIDEO = restaurant.Video?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    FACEBOOK = restaurant.Facebook?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    LINE = restaurant.Line?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    OPEN_TIME = restaurant.OpenTime?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    IS_CAR_PARK = restaurant.CarPark == 1
                };

                var user = _dbContext.User.FirstOrDefault(f => f.UserId == restaurant.OwnerId);
                if (user != null)
                {
                    res.OWNER = string.Format("{0} {1}",
                        user.FirstName?.Trim() ?? string.Empty,
                        user.LastName?.Trim() ?? string.Empty).Trim();
                }

                if (media != null)
                {
                    medias = _dbContext.UploadFileUtilities.GetMedias(
                        UploadEnum.RESTAURANT.GetString(),
                        restaurant.ResId.ToString(),
                        media.Path)
                        //=>Random .OrderBy(o => Guid.NewGuid())
                        .Take(param.limit)
                        .ToList();

                    medias.Add(media);

                    if (medias != null && medias.Count > 0)
                    {
                        medias.ForEach(m =>
                        {
                            var image = _env.GetImageThumbnail(_context, m.Path, ImageExtension.DEFAULT_IMAGE);

                            if (image != null)
                            {
                                res.IMAGE.Add(image.image);
                                res.IMAGE_SM.Add(image.imageSM);
                                res.IMAGE_MD.Add(image.imageMD);
                                res.IMAGE_LG.Add(image.imageLG);
                            }
                        });
                    }
                }
                else
                {
                    var image = _env.GetImageThumbnail(_context, ImageExtension.DEFAULT_IMAGE);
                    res.IMAGE.Add(image.image);
                    res.IMAGE_SM.Add(image.imageSM);
                    res.IMAGE_MD.Add(image.imageMD);
                    res.IMAGE_LG.Add(image.imageLG);
                }


                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:200_SUCCESS");
                this.response.data = res;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Detail.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Menu(RestaurantParameter param)
        {
            RestaurantMenuResponse response = new RestaurantMenuResponse();
            response.TITLE = "รายการอาหาร";

            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Menu:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var menus = _dbContext.RestaurantMenu.Where(w => w.ResId == param.RESTAURANT_ID)
                    .ToList();

                if (menus != null && menus.Count > 0)
                {
                    response.MENU = (from menu in menus
                                     let image = _env.GetImageThumbnail(_context, menu.Thumbnail, ImageExtension.DEFAULT_IMAGE)
                                     select new MenuResponse
                                     {
                                         MENU_ID_1 = menu.MenuId,
                                         MENU_ID_2 = menu.MenuId,
                                         RESTAURANT_ID = menu.ResId,
                                         NAME = menu.NameTh.Trim()?.DecodeSpacialCharacters() ?? string.Empty,
                                         VIEWER = menu.Viewer,
                                         IMAGE = (image != null) ? image.image : string.Empty,
                                         IMAGE_SM = (image != null) ? image.imageSM : string.Empty,
                                         IMAGE_MD = (image != null) ? image.imageMD : string.Empty,
                                         IMAGE_LG = (image != null) ? image.imageLG : string.Empty,
                                         PRICE = menu.Price,
                                         UNIT = "บาท"
                                     }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Menu:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Menu:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = response.MENU.Count;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Menu.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Menu:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> MenuDetail(RestaurantMenuDetailParameter param)
        {
            try
            {
                RestaurantMenuDetailResponse resMenu = null;

                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "MenuDetail:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var menu = _dbContext.RestaurantMenu.Where(w => w.ResId == param.RESTAURANT_ID &&
                    w.MenuId == param.MENU_ID)
                    .FirstOrDefault();

                if (menu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "MenuDetail:404_RESTAURANT_MENU_NOT_FOUND");

                    return this.response;
                }

                //=>Thumbnail
                var media = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.RESTAURANT_MENU.GetString(),
                    menu.MenuId.ToString());


                List<Media> medias = new List<Media>();

                //=>Sum vote
                var scores = _dbContext.Vote.Where(w => w.RefId == param.MENU_ID.ToString() &&
                    w.SystemName == UploadEnum.RESTAURANT_MENU.GetString())
                    .Select(s => new
                    {
                        score = s.Score
                    }).ToList();

                double avg = Math.Round(scores.Count == 0 ? 0 : (double)scores.Sum(s => s.score) / (double)scores.Count, 2);

                resMenu = new RestaurantMenuDetailResponse()
                {
                    MENU_ID_S1 = menu.MenuId,
                    MENU_ID_S2 = menu.MenuId,
                    RESTAURANT_ID = menu.ResId,
                    COUNTRY_ID = menu.CountryId,
                    CULTURE_ID = menu.CultureId,
                    NAME = menu.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    VIEWER = menu.Viewer,
                    COOKING_FOOD = menu.CookingFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    LEGEND_STATUS = menu.LegendStatus == 1,
                    RATING = avg,
                    PRICE = menu.Price,
                    UNIT = "บาท"
                };

                if (media != null)
                {
                    medias = _dbContext.UploadFileUtilities.GetMedias(
                        UploadEnum.RESTAURANT_MENU.GetString(),
                        menu.MenuId.ToString(),
                        media.Path)
                        //=>Random .OrderBy(o => Guid.NewGuid())
                        .Take(param.limit)
                        .ToList();

                    if (medias != null && medias.Count > 0)
                    {
                        medias.ForEach(m =>
                        {
                            var image = _env.GetImageThumbnail(_context, m.Path, ImageExtension.DEFAULT_IMAGE);

                            if (image != null)
                            {
                                resMenu.IMAGE.Add(image.image);
                                resMenu.IMAGE_SM.Add(image.imageSM);
                                resMenu.IMAGE_MD.Add(image.imageMD);
                                resMenu.IMAGE_LG.Add(image.imageLG);
                            }
                        });
                    }
                }
                else
                {
                    var image = _env.GetImageThumbnail(_context, ImageExtension.DEFAULT_IMAGE);
                    resMenu.IMAGE.Add(image.image);
                    resMenu.IMAGE_SM.Add(image.imageSM);
                    resMenu.IMAGE_MD.Add(image.imageMD);
                    resMenu.IMAGE_LG.Add(image.imageLG);
                }


                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "MenuDetail:200_SUCCESS");
                this.response.data = resMenu;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.Detail.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Legend(RestaurantMenuDetailParameter param)
        {
            LegendResponse legend = new LegendResponse();

            legend.TITLE = "ตำนานอาหาร";
            legend.LEGEND = new List<Models.Responses.Frontend.Legend>();

            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var restaurantMenu = _dbContext.RestaurantMenu.Where(w => w.ResId == param.RESTAURANT_ID &&
                    w.MenuId == param.MENU_ID)
                    .FirstOrDefault();

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:404_RESTAURANT_MENU_NOT_FOUND");

                    return this.response;
                }

                var _legend = _dbContext.Legend.Where(w => w.Code == restaurantMenu.Code &&
                    w.LegendType == (int)LegendEnum.LEGEND_RESTAURANT)
                    .ToList();

                if (_legend != null && _legend.Count < 1)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:204_NO_CONTENT");
                    this.response.total = legend.LEGEND.Count;
                    this.response.data = legend;

                    return this.response;
                }

                legend.LEGEND = (from lg in _legend.Take(1)
                                 let image = !string.IsNullOrEmpty(lg.Thumbnail) ? _env.GetImageThumbnail(_context, lg.Thumbnail, ImageExtension.DEFAULT_IMAGE) : null
                                 select new Models.Responses.Frontend.Legend
                                 {
                                     LEGEND_ID = lg.Id,
                                     DESCRIPTION = lg.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                     NAME = restaurantMenu.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                     IMAGE = image != null ? image.image : string.Empty,
                                     IMAGE_SM = image != null ? image.imageSM : string.Empty,
                                     IMAGE_MD = image != null ? image.imageMD : string.Empty,
                                     IMAGE_LG = image != null ? image.imageLG : string.Empty
                                 }).ToList();

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:200_SUCCESS");
                this.response.total = legend.LEGEND.Count;
                this.response.data = legend;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Legend.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
        //public async Task<BaseResponse> MenuIngredient(RestaurantMenuIngredientParameter param)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return await Task.Run(() => this.response);
        //}

        public async Task<BaseResponse> Vote(RestaurantDetailVoteParameter param)
        {
            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                        .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                Vote vote = _dbContext.Vote.Where(w => w.ClientId == param.DEVICE_ID &&
                    w.RefId == param.RESTAURANT_ID.ToString() &&
                    w.SystemName == UploadEnum.RESTAURANT.GetString())
                    .FirstOrDefault();

                bool IsNew = true;

                var now = DateTime.Now;
                if (vote == null)
                {
                    //=>ADD
                    vote = new Vote
                    {
                        ClientId = param.DEVICE_ID,
                        RefId = param.RESTAURANT_ID.ToString(),
                        Score = (int)param.SCORE,
                        SystemName = UploadEnum.RESTAURANT.GetString(),
                        CreatedDate = now,
                        UpdatedDate = now
                    };
                }
                else
                {
                    //=>EDIT
                    vote.Score = (int)param.SCORE;
                    vote.UpdatedDate = now;
                    IsNew = false;
                }

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    if (IsNew)
                    {
                        _dbContext.Vote.Add(vote);
                    }
                    else
                    {
                        _dbContext.Vote.Update(vote);
                    }

                    _dbContext.SaveChanges();

                });

                //=>Sum vote
                var scores = _dbContext.Vote.Where(w => w.RefId == param.RESTAURANT_ID.ToString() &&
                    w.SystemName == UploadEnum.RESTAURANT.GetString())
                    .Select(s => new
                    {
                        score = s.Score
                    }).ToList();

                double avg = Math.Round((double)scores.Sum(s => s.score) / (double)scores.Count, 2);

                if (result.success)
                {

                    this.response.success = true;
                    this.response.statusCode = IsNew ? (int)HttpStatusCode.Created : (int)HttpStatusCode.OK;
                    this.response.message = IsNew ? _message.GetMessage(ROOT_MESSAGE, "Vote:201_SUCCESS_ADD")
                        : _message.GetMessage(ROOT_MESSAGE, "Vote:200_SUCCESS_EDIT");
                    this.response.data = new
                    {
                        Id = restaurant.ResId,
                        restaurantId = restaurant.ResId,
                        rating = avg
                    };
                }
                else
                {
                    _logger.LogError("Frontend.RestaurantService.Vote.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:400_BAD_REQUEST");
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Vote.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> VoteExist(RestaurantDetailParameter param)
        {
            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                                     .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                Vote vote = _dbContext.Vote.Where(w => w.ClientId == param.DEVICE_ID &&
                    w.RefId == param.RESTAURANT_ID.ToString() &&
                    w.SystemName == UploadEnum.RESTAURANT.GetString())
                    .FirstOrDefault();

                if (vote == null)
                {
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:200_NOT_EXISTS");
                    this.response.data = new
                    {
                        Id = restaurant.ResId,
                        restaurantId = restaurant.ResId,
                        voteExist = false,
                        voteScore = (double)0
                    };
                }
                else
                {
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:200_EXISTS");
                    this.response.data = new
                    {
                        Id = restaurant.ResId,
                        restaurantId = restaurant.ResId,
                        voteExist = true,
                        voteScore = (double)vote.Score
                    };
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.VoteExist.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Comments(RestaurantDetailCommentParameter param)
        {
            List<CommentResponse> comments = new List<CommentResponse>();

            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var _comments = _dbContext.Comment.Where(w => w.RefId == param.RESTAURANT_ID.ToString() &&
                    w.SystemName == UploadEnum.COMMENT.GetString())
                    .ToList();


                comments = (from c in _comments
                            select new CommentResponse
                            {
                                CMT_ID = c.CmtId,
                                USER_ID = c.UserId,
                                COMMENT = c.Comment1?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                CREATE_DATE = c.CreatedDate.Value,
                            }).ToList();

                if (comments.Count > 0)
                {

                    //=>Find Comment Image
                    var cmtIds = comments.Select(s => s.CMT_ID.ToString()).ToList();
                    var medias = _dbContext.Media.Where(w => cmtIds.Contains(w.RefId) &&
                        w.SystemName == UploadEnum.COMMENT.GetString()).ToList();

                    if (medias != null && medias.Count > 0)
                    {
                        comments.ForEach(f =>
                        {
                            var find = medias.FirstOrDefault(ff => ff.RefId == f.CMT_ID.ToString());

                            if (find != null)
                            {
                                var image = _env.GetImageThumbnail(_context, find.Path, ImageExtension.DEFAULT_IMAGE);

                                f.IMAGE = image.image;
                                f.IMAGE_SM = image.imageSM;
                                f.IMAGE_MD = image.imageMD;
                                f.IMAGE_LG = image.imageLG;
                            }
                        });
                    }

                    var userIds = _comments.Select(s => s.UserId).ToList();
                    var users = _dbContext.UserUtilities.GetUsers((User user) => new
                    {
                        USER_ID = user.UserId,
                        FIRST_NAME = user.FirstName,
                        LAST_NAME = user.LastName,
                        AVATAR = user.Avatar,
                    }, userIds);

                    comments.ForEach(f =>
                    {
                        var find = users.FirstOrDefault(ff => ff.USER_ID == f.USER_ID);
                        if (find != null)
                        {
                            f.FULL_NAME = string.Format("{0} {1}",
                                find.FIRST_NAME.Trim(),
                                find.LAST_NAME.Trim()).Trim();
                        }

                        var avatar = _env.GetImageThumbnail(_context, find?.AVATAR ?? string.Empty, ImageExtension.DEFAULT_AVATAR);

                        f.IMAGE_AVATAR = avatar.image;
                        f.IMAGE_AVATAR_SM = avatar.imageSM;
                        f.IMAGE_AVATAR_MD = avatar.imageMD;
                        f.IMAGE_AVATAR_LG = avatar.imageLG;
                    });
                }

                if (comments.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:204_NO_CONTENT");
                }
                this.response.success = true;
                this.response.total = comments.Count;
                this.response.data = new
                {
                    comments = comments
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Comments.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Review(RestaurantDetailReviewParameter param)
        {
            try
            {
                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                        .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var now = DateTime.Now;
                var comment = new Comment
                {
                    Comment1 = param.COMMENT?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    RefId = restaurant.ResId.ToString(),
                    SystemName = UploadEnum.COMMENT.ToString(),
                    UserId = _authen.User.USER_ID,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;
                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Comment.Add(comment);
                    _dbContext.SaveChanges();

                    if (param.IMAGES != null)
                    {
                        string fileName = param.IMAGES.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.COMMENT.GetString(),
                            fileName,
                            comment.CmtId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.IMAGES);

                        comment.ImageStatus = 1;
                        _dbContext.Comment.Update(comment);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:201_SUCCESS");
                    this.response.data = null;
                }
                else
                {
                    _logger.LogError("Frontend.RestaurantService.Review.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Review.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Promotion(RestaurantPromotionParameter param)
        {
            try
            {
                List<PromotionResponse> response = new List<PromotionResponse>();

                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RESTAURANT_ID)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Promotion:404_RESTAURANT_NOT_FOUND");

                    return this.response;
                }

                var now = DateTime.Now;

                var promotions = _dbContext.Promotion.Where(w => w.ResId == param.RESTAURANT_ID)
                    .Where(w => w.Flag != (int)PromotionEnum.HIDE)
                    .Where(w => (w.Flag == (int)PromotionEnum.FREEZE || (w.Flag == (int)PromotionEnum.USE_ESTIMATE &&
                        w.StartDate != null &&
                        w.EndDate != null &&
                        w.StartDate <= now && w.EndDate >= now)))
                    .ToList();


                //promotions = (from o in promotions
                //              let isEstimate = o.Flag == (int)PromotionEnum.USE_ESTIMATE
                //              where (o.Flag == (int)PromotionEnum.FREEZE || (isEstimate && o.StartDate >= now && o.EndDate <= now))
                //              select o).ToList();

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
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Promotion:200_SUCCESS");
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Promotion:204_NO_CONTENT");
                }

                this.response.success = true;
                this.response.total = response.Count;
                this.response.data = response;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.RestaurantService.Promotion.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Promotion:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }


            return await Task.Run(() => this.response);
        }
    }
}
