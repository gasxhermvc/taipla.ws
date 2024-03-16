using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
using Taipla.Webservice.Models.Parameters.Frontend.Food;
using Taipla.Webservice.Models.Responses.Frontend;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class FoodCenterService : IFoodCenterService
    {
        private readonly IConfiguration _configuration;
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<ClientInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private readonly MessageFactory _message;
        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:FoodController";

        public FoodCenterService(IConfiguration configuration,
            TAIPLA_DbContext dbContext
            , IAuthenticationService<ClientInfo> authen
            , IHttpContextAccessor context
            , ILoggerFactory logger
            , IWebHostEnvironment env
            , MessageFactory messageFactory)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
            _message = messageFactory;
        }
        public async Task<BaseResponse> Detail(FoodDetailParameter param)
        {
            try
            {
                FoodDetailResponse food = null;

                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                    .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                //=>Thumbnail
                var media = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.FOOD_CENTER.GetString(),
                    foodCenter.FoodId.ToString());


                List<Media> medias = new List<Media>();

                //=>Sum vote
                var scores = _dbContext.Vote.Where(w => w.RefId == param.foodId.ToString() &&
                    w.SystemName == UploadEnum.FOOD_CENTER.GetString())
                    .Select(s => new
                    {
                        score = s.Score
                    }).ToList();

                double avg = Math.Round(scores.Count == 0 ? 0 : (double)scores.Sum(s => s.score) / (double)scores.Count, 2);

                food = new FoodDetailResponse()
                {
                    FOOD_ID_S1 = foodCenter.FoodId,
                    FOOD_ID_S2 = foodCenter.FoodId,
                    COUNTRY_ID = foodCenter.CountryId,
                    CULTURE_ID = foodCenter.CultureId,
                    NAME = foodCenter.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    VIEWER = foodCenter.Viewer,
                    COOKING_FOOD = foodCenter.CookingFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    INGREDEINTS = foodCenter.Ingredient?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    LEGEND_STATUS = foodCenter.LegendStatus == 1,
                    RATING = avg
                };

                if (media != null)
                {
                    medias = _dbContext.UploadFileUtilities.GetMedias(
                        UploadEnum.FOOD_CENTER.GetString(),
                        foodCenter.FoodId.ToString(),
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
                                food.IMAGE.Add(image.image);
                                food.IMAGE_SM.Add(image.imageSM);
                                food.IMAGE_MD.Add(image.imageMD);
                                food.IMAGE_LG.Add(image.imageLG);
                            }
                        });
                    }
                }
                else
                {
                    var image = _env.GetImageThumbnail(_context, ImageExtension.DEFAULT_IMAGE);
                    food.IMAGE.Add(image.image);
                    food.IMAGE_SM.Add(image.imageSM);
                    food.IMAGE_MD.Add(image.imageMD);
                    food.IMAGE_LG.Add(image.imageLG);
                }


                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Detail:200_SUCCESS");
                this.response.data = food;
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

        public async Task<BaseResponse> Images(FoodDetailParameter param)
        {
            ImageListResponse images = new ImageListResponse();

            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == w.FoodId)
                    .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Images:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                //=>Thumbnail
                var media = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.FOOD_CENTER.GetString(),
                    foodCenter.FoodId.ToString());

                if (media != null)
                {
                    var medias = _dbContext.UploadFileUtilities.GetMedias(
                        UploadEnum.FOOD_CENTER.GetString(),
                        foodCenter.FoodId.ToString(),
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
                                images.IMAGE.Add(image.image);
                                images.IMAGE_SM.Add(image.imageSM);
                                images.IMAGE_MD.Add(image.imageMD);
                                images.IMAGE_LG.Add(image.imageLG);
                            }
                        });
                    }
                }


                if (images != null && images.IMAGE.Count > 0)
                {


                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Images:200_SUCCESS");
                    this.response.total = images.IMAGE.Count;
                    this.response.data = images;
                }
                else
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Images:204_NO_CONTENT");
                    this.response.total = images.IMAGE.Count;
                    this.response.data = images;
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.Images.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Images:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Legend(FoodDetailParameter param)
        {
            LegendResponse legend = new LegendResponse();

            legend.TITLE = "ตำนานอาหาร";
            legend.LEGEND = new List<Models.Responses.Frontend.Legend>();

            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                    .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                var _legend = _dbContext.Legend.Where(w => w.Code == foodCenter.Code &&
                    w.LegendType == (int)LegendEnum.LEGEND_FOOD)
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
                                     NAME = foodCenter.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
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
                _logger.LogError("Frontend.FoodCenterService.Legend.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Legend:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Recommendation(FoodDetailParameter param)
        {
            List<FoodResponse> recommend = new List<FoodResponse>();

            try
            {
                var recommendPath = _configuration.GetSection("Application:Recommendation:FoodCenter").Value;

                string readFile = String.Format($"{recommendPath}/{param.foodId}.json");
                if (!System.IO.File.Exists(readFile))
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Recommendation:204_NO_CONTENT");
                    this.response.total = recommend.Count;
                    this.response.data = new
                    {
                        title = "รายการอาหารอื่นที่คล้ายกัน",
                        foods = recommend
                    };

                    return this.response;
                }

                var foodIds = System.IO.File.ReadAllText(readFile).Split('|')
                    .Where(w => !string.IsNullOrEmpty(w))
                    .Select(s => int.Parse(s))
                    .Where(w => w != param.foodId)
                    .ToList();

                var foods = _dbContext.FoodCenter.Where(w => foodIds.Contains(w.FoodId))
                    .Take(param.limit)
                    .ToList();

                if (foods != null && foods.Count > 0)
                {
                    recommend = (from food in foods
                                 let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                 select new FoodResponse
                                 {
                                     FOOD_ID_S1 = food.FoodId,
                                     FOOD_ID_S2 = food.FoodId,
                                     COUNTRY_ID = food.CountryId,
                                     CULTURE_ID = food.CultureId,
                                     NAME = food.NameTh.Trim(),
                                     IMAGE = image != null ? image.image : string.Empty,
                                     IMAGE_SM = image != null ? image.imageSM : string.Empty,
                                     IMAGE_MD = image != null ? image.imageMD : string.Empty,
                                     IMAGE_LG = image != null ? image.imageLG : string.Empty,
                                     VIEWER = food.Viewer
                                 }).ToList();

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Recommendation:200_SUCCESS");
                    this.response.total = recommend.Count;
                    this.response.data = new
                    {
                        title = "รายการอาหารอื่นที่คล้ายกัน",
                        foods = recommend
                    };
                }
                else
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Recommendation:204_NO_CONTENT");
                    this.response.total = recommend.Count;
                    this.response.data = new
                    {
                        title = "รายการอาหารอื่นที่คล้ายกัน",
                        foods = recommend
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.Recommendation.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Recommendation:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Vote(FoodDetailVoteParameter param)
        {
            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                        .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                Vote vote = _dbContext.Vote.Where(w => w.ClientId == param.deviceId &&
                    w.RefId == param.foodId.ToString() &&
                    w.SystemName == UploadEnum.FOOD_CENTER.GetString())
                    .FirstOrDefault();

                bool IsNew = true;

                var now = DateTime.Now;
                if (vote == null)
                {
                    //=>ADD
                    vote = new Vote
                    {
                        ClientId = param.deviceId,
                        RefId = param.foodId.ToString(),
                        Score = (int)param.score,
                        SystemName = UploadEnum.FOOD_CENTER.GetString(),
                        CreatedDate = now,
                        UpdatedDate = now
                    };
                }
                else
                {
                    //=>EDIT
                    vote.Score = (int)param.score;
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
                var scores = _dbContext.Vote.Where(w => w.RefId == param.foodId.ToString() &&
                    w.SystemName == UploadEnum.FOOD_CENTER.GetString())
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
                        Id = foodCenter.FoodId,
                        foodId = foodCenter.FoodId,
                        rating = avg
                    };
                }
                else
                {
                    _logger.LogError("Frontend.FoodCenterService.Vote.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:400_BAD_REQUEST");
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.Vote.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Vote:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> VoteExist(FoodDetailVoteExistParameter param)
        {
            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                                     .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                Vote vote = _dbContext.Vote.Where(w => w.ClientId == param.deviceId &&
                    w.RefId == param.foodId.ToString() &&
                    w.SystemName == UploadEnum.FOOD_CENTER.GetString())
                    .FirstOrDefault();

                if (vote == null)
                {
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:200_NOT_EXISTS");
                    this.response.data = new
                    {
                        Id = foodCenter.FoodId,
                        foodId = foodCenter.FoodId,
                        voteExist = false,
                        voteScore = (double)0
                    };
                }
                else
                {
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:200_EXISTS");
                    this.response.data = new
                    {
                        Id = foodCenter.FoodId,
                        foodId = foodCenter.FoodId,
                        voteExist = true,
                        voteScore = (double)vote.Score
                    };
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.VoteExist.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "VoteExist:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Comments(FoodDetailCommentParameter param)
        {
            List<CommentResponse> comments = new List<CommentResponse>();

            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                    .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                var _comments = _dbContext.Comment.Where(w => w.RefId == param.foodId.ToString() &&
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
                _logger.LogError("Frontend.FoodCenterService.Comments.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Comments:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        [Authorize]
        public async Task<BaseResponse> Review(FoodDetailReviewParameter param)
        {
            try
            {
                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.foodId)
                        .FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:404_FOOD_CENTER_NOT_FOUND");

                    return this.response;
                }

                var now = DateTime.Now;
                var comment = new Comment
                {
                    Comment1 = param.comment?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    RefId = foodCenter.FoodId.ToString(),
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

                    if (param.images != null)
                    {
                        string fileName = param.images.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.COMMENT.GetString(),
                            fileName,
                            comment.CmtId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.images);

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
                    _logger.LogError("Frontend.FoodCenterService.Review.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.FoodCenterService.Review.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Review:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
    }
}
