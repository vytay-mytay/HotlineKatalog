using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.ResponseModels;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HotlineKatalog.Services.AutoMapperConfig
{
    public class AutoMapperProfileConfiguration : AutoMapper.Profile
    {
        public AutoMapperProfileConfiguration()
        : this("MyProfile")
        {
        }

        protected AutoMapperProfileConfiguration(string profileName)
        : base(profileName)
        {
            CreateMap<Good, GoodResponseModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(t => t.Category, opt => opt.MapFrom(x => x.Category))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(t => t.Prices, opt => opt.MapFrom(x => x.Prices))
                .ForMember(t => t.Producer, opt => opt.MapFrom(x => x.Producer))
                .ForMember(t => t.Specification, opt => opt.MapFrom(x => x.Specification));

            CreateMap<Category, CategoryResponseModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<Producer, ProducerResponseModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<Shop, ShopResponseModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<Specification, SpecificationResponseModel>()
                .ForMember(t => t.Specification, opt => opt.MapFrom(x => JsonConvert.DeserializeObject<Dictionary<string, string>>(x.Json)));

            CreateMap<Price, PriceResponseModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(t => t.Date, opt => opt.MapFrom(x => x.Date))
                .ForMember(t => t.Shop, opt => opt.MapFrom(x => x.Shop))
                .ForMember(t => t.Value, opt => opt.MapFrom(x => x.Value))
                .ForMember(t => t.Url, opt => opt.Ignore());    // aftermap

            //CreateMap<Profile, UserProfileResponseModel>()
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar))
            //    .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User != null ? x.User.Email : ""))
            //    .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => x.User != null && !x.User.IsActive))
            //    .ForMember(t => t.Urls, opt => opt.MapFrom(x => x.Urls.ToList()))
            //    .ForMember(t => t.Achievements, opt => opt.MapFrom(x => x.Achievements.ToList()))
            //    .ForMember(t => t.Role, opt => opt.Ignore())
            //    .ForMember(t => t.Username, opt => opt.MapFrom(x => x.User != null ? x.User.UserName : ""))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info ?? new ProfileInfo()))
            //    .ForMember(t => t.IsOnline, opt => opt.MapFrom(x => x.User.IsOnline))
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.User.ReceivedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.User.CreatedBlocks.AnyOrNull()))
            //    .ForMember(t => t.Bio, opt => opt.MapFrom(x => x.IsHidden ? null : x.Bio))
            //    .ForMember(t => t.Nationality, opt => opt.MapFrom(x => x.IsHidden ? null : x.Nationality))
            //    .ForMember(t => t.Age, opt => opt.MapFrom(x => x.IsHidden ? null : x.Age))
            //    .ForMember(t => t.DateOfBirth, opt => opt.MapFrom(x => x.IsHidden ? null : x.DateOfBirth.ToISO()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.User != null && x.User.UserDeletedAt.HasValue))
            //    .ForMember(t => t.IsComplete, opt => opt.MapFrom(x => x.CompletedAt.HasValue));

            //CreateMap<Profile, UserProfileChatResponseModel>()
            //    .IncludeBase<Profile, UserProfileResponseModel>()
            //    .ForMember(t => t.ChatId, opt => opt.MapFrom(x => x.User != null && x.User.Chats.Any() ? (int?)x.User.Chats.FirstOrDefault().ChatId : null))
            //    .ForMember(t => t.LocalChatId, opt => opt.MapFrom(x => x.User != null && x.User.Chats.Any() ? x.User.Chats.FirstOrDefault().LocalChatId : null));

            //CreateMap<PreferencesRequestModel, Preferences>()
            //   .ForMember(t => t.Id, opt => opt.Ignore())
            //   .ForMember(t => t.User, opt => opt.Ignore());

            //CreateMap<Preferences, StoryTypesRequestModel>();

            //CreateMap<Preferences, PreferencesResponseModel>();

            //CreateMap<PreferencesRequestModel, PreferencesResponseModel>();

            //CreateMap<Image, ImageResponseModel>();

            //CreateMap<ImageResponseModel, Image>()
            //    .ForMember(t => t.Id, opt => opt.Ignore());

            //CreateMap<UserDevice, UserDeviceResponseModel>()
            //    .ForMember(t => t.AddedAt, opt => opt.MapFrom(src => (DateTimeOffset)src.AddedAt));

            //CreateMap<Message, ChatMessageBaseResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Type, opt => opt.MapFrom(x => x.MessageType))
            //    .ForMember(t => t.Image, opt => opt.MapFrom(x => x.Image))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Status, opt => opt.MapFrom(x => x.MessageStatus));

            //CreateMap<Message, ChatMessageResponseModel>()
            //    .IncludeBase<Message, ChatMessageBaseResponseModel>();

            //CreateMap<UrlRequestModel, AuthorUrl>();

            //CreateMap<AuthorAchievementRequestModel, AuthorAchievement>();

            //CreateMap<AuthorAchievement, AuthorAchievementResponseModel>()
            //    .ForMember(t => t.Date, opt => opt.MapFrom(x => x.Date.ToISO()));

            //CreateMap<AuthorUrl, AuthorUrlResponseModel>();

            //CreateMap<Diary, DiaryResponseModel>()
            //    .ForMember(t => t.Date, opt => opt.MapFrom(x => x.Date.ToISO()))
            //    .ForMember(t => t.IsPublished, opt => opt.MapFrom(x => x.PublishedAt.HasValue));

            //CreateMap<CreateDiaryRequestModel, Diary>()
            //   .ForMember(t => t.Id, opt => opt.Ignore())
            //   .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
            //   .ForMember(t => t.SpecificUsers, opt => opt.Ignore())
            //   .ForMember(t => t.PublishedAt, opt => opt.MapFrom(x => x.ShouldBePublished ? (DateTime?)DateTime.UtcNow : null));

            //CreateMap<PoemRequestModel, Post>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
            //    .ForMember(t => t.BackgroundColor, opt => opt.MapFrom(x => x.BackgroundColor))
            //    .ForMember(t => t.Hashtags, opt => opt.Ignore())
            //    .ForMember(t => t.SpecificUsers, opt => opt.Ignore())
            //    .ForMember(t => t.MentionedUsers, opt => opt.Ignore())
            //    .ForMember(t => t.Id, opt => opt.Ignore())
            //    .ForMember(t => t.Signature, opt => opt.Ignore());

            //CreateMap<Post, PostResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Audio, opt => opt.MapFrom(x => x.Audio))
            //    .ForMember(t => t.Category, opt => opt.MapFrom(x => x.StoryType))
            //    .ForMember(t => t.MentionedUsers, opt => opt.MapFrom(x => x.MentionedUsers))
            //    .ForMember(t => t.SpecificUsers, opt => opt.MapFrom(x => x.SpecificUsers))
            //    .ForMember(t => t.IsSaved, opt => opt.MapFrom(x => x.ReadLaters.Any() || x.Favourites.Any()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.DeletedAt != null));

            //CreateMap<Post, FeedItemResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Audio, opt => opt.MapFrom(x => x.Audio))
            //    .ForMember(t => t.Category, opt => opt.MapFrom(x => x.StoryType))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info))
            //    .ForMember(t => t.MentionedUsers, opt => opt.MapFrom(x => x.MentionedUsers))
            //    .ForMember(t => t.IsSaved, opt => opt.MapFrom(x => x.ReadLaters.Any() || x.Favourites.Any()))
            //    .ForMember(t => t.SpecificUsers, opt => opt.MapFrom(x => x.SpecificUsers))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Post));

            //CreateMap<Advertisment, FeedItemResponseModel>()
            //    .ForMember(t => t.Creator, opt => opt.MapFrom(x => x))
            //    .ForMember(t => t.Type, opt => opt.MapFrom(x => FeedItemType.Advertisment))
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Image, opt => opt.MapFrom(x => x.Content))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Advertisment));

            //CreateMap<Advertisment, PostUserResponseModel>()
            //     .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar))
            //     .ForMember(t => t.Id, opt => opt.Ignore());

            //CreateMap<Post, UserPublicationResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Audio, opt => opt.MapFrom(x => x.Audio))
            //    .ForMember(t => t.Category, opt => opt.MapFrom(x => x.StoryType))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Post))
            //    .ForMember(t => t.MentionedUsers, opt => opt.MapFrom(x => x.MentionedUsers))
            //    .ForMember(t => t.IsSaved, opt => opt.MapFrom(x => x.Favourites.Any() || x.ReadLaters.Any()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.DeletedAt != null));

            //CreateMap<Diary, UserPublicationResponseModel>()
            //    .ForMember(t => t.IsPublished, opt => opt.MapFrom(x => x.PublishedAt != null))
            //    .ForMember(t => t.Creator, opt => opt.MapFrom(x => x.User))
            //    .ForMember(t => t.Date, opt => opt.MapFrom(x => x.Date.ToISO()))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Diary))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info))
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.IsSaved, opt => opt.MapFrom(x => x.Favourites.Any()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.DeletedAt != null));

            //CreateMap<int, MentionedUser>()
            //    .ForMember(t => t.UserId, opt => opt.MapFrom(x => x));

            //CreateMap<int, SpecificUser>()
            //    .ForMember(t => t.UserId, opt => opt.MapFrom(x => x));

            //CreateMap<string, Hashtag>()
            //    .ForMember(t => t.Tag, opt => opt.MapFrom(x => x));

            //CreateMap<Hashtag, HashtagPost>()
            //    .ForMember(t => t.Hashtag, opt => opt.MapFrom(x => x.Id == 0 ? null : x))
            //    .ForMember(t => t.HashtagId, opt => opt.MapFrom(x => x.Id));

            //CreateMap<HashtagPost, HashtagResponseModel>()
            //    .ForMember(t => t.Id, opt => opt.MapFrom(x => x.HashtagId))
            //    .ForMember(t => t.Tag, opt => opt.MapFrom(x => x.Hashtag.Tag));

            //CreateMap<Hashtag, HashtagResponseModel>();

            //CreateMap<Profile, PostUserResponseModel>()
            //    .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.User.UserName))
            //    .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.FirstName))
            //    .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.LastName))
            //    .ForMember(t => t.Role, opt => opt.MapFrom(x => x.User.UserRoles.Any() ? x.User.UserRoles.First().Role : null))
            //    .ForMember(t => t.IsOnline, opt => opt.MapFrom(x => x.User.IsOnline))
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.User.ReceivedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.User.CreatedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.User.UserDeletedAt.HasValue));

            //CreateMap<ApplicationUser, PostUserResponseModel>()
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Profile.Avatar))
            //    .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.Profile.FirstName))
            //    .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.Profile.LastName))
            //    .ForMember(t => t.Role, opt => opt.MapFrom(x => x.UserRoles.Any() ? x.UserRoles.First().Role : null))
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.ReceivedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.CreatedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.UserDeletedAt.HasValue));

            //CreateMap<Subscription, FollowingResponseModel>()
            //   .ForMember(t => t.Id, opt => opt.MapFrom(x => x.ToId))
            //    .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.ToUser.UserName))
            //    .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.ToUser.Profile.FirstName))
            //    .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.ToUser.Profile.LastName))
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.ToUser.Profile.Avatar))
            //    .ForMember(t => t.SubscribedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.IsPinned, opt => opt.MapFrom(x => x.PinnedAt.HasValue))
            //    .ForMember(t => t.Role, opt => opt.MapFrom(x => x.ToUser.UserRoles.Any() ? x.ToUser.UserRoles.First().Role : null))
            //    .ForMember(t => t.IsOnline, opt => opt.MapFrom(x => x.ToUser.IsOnline))
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.ToUser.ReceivedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.ToUser.CreatedBlocks.AnyOrNull()))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.ToUser.UserDeletedAt.HasValue));

            //CreateMap<Subscription, FollowerResponseModel>()
            //   .ForMember(t => t.Id, opt => opt.MapFrom(x => x.FromId))
            //   .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.FromUser.UserName))
            //   .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.FromUser.Profile.FirstName))
            //   .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.FromUser.Profile.LastName))
            //   .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.FromUser.Profile.Avatar))
            //   .ForMember(t => t.SubscribedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //   .ForMember(t => t.Role, opt => opt.MapFrom(x => x.FromUser.UserRoles.Any() ? x.FromUser.UserRoles.First().Role : null))
            //   .ForMember(t => t.IsOnline, opt => opt.MapFrom(x => x.FromUser.IsOnline))
            //   .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.FromUser.ReceivedBlocks.AnyOrNull()))
            //   .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.FromUser.CreatedBlocks.AnyOrNull()))
            //   .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.FromUser.UserDeletedAt.HasValue));

            //CreateMap<Profile, ChatUserResponseModel>()
            //    .IncludeBase<Profile, PostUserResponseModel>()
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.User.ReceivedBlocks.Any()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.User.CreatedBlocks.Any()))
            //    .ForMember(t => t.ChatId, opt => opt.MapFrom(x => x.User.Chats.Any() ? (int?)x.User.Chats.First().ChatId : null))
            //    .ForMember(t => t.LocalChatId, opt => opt.MapFrom(x => x.User.Chats.Any() ? x.User.Chats.First().LocalChatId : null));

            //CreateMap<ApplicationUser, ChatUserResponseModel>()
            //    .IncludeBase<ApplicationUser, PostUserResponseModel>()
            //    .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.ReceivedBlocks.Any()))
            //    .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.CreatedBlocks.Any()))
            //    .ForMember(t => t.ChatId, opt => opt.MapFrom(x => x.Chats.Any() ? (int?)x.Chats.First().ChatId : null))
            //    .ForMember(t => t.LocalChatId, opt => opt.MapFrom(x => x.Chats.Any() ? x.Chats.First().LocalChatId : null));

            //CreateMap<File, ContentResponseModel>()
            //    .ForMember(t => t.OriginalPath, opt => opt.MapFrom(x => x.Path))
            //    .ForMember(t => t.CompactPath, opt => opt.MapFrom(x => x.Path));

            //CreateMap<QuotesRequestModel, Post>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
            //    .ForMember(t => t.BackgroundColor, opt => opt.MapFrom(x => x.BackgroundColor))
            //    .ForMember(t => t.Hashtags, opt => opt.Ignore())
            //    .ForMember(t => t.SpecificUsers, opt => opt.Ignore())
            //    .ForMember(t => t.MentionedUsers, opt => opt.Ignore())
            //    .ForMember(t => t.Id, opt => opt.Ignore())
            //    .ForMember(t => t.Resources, opt => opt.Ignore())
            //    .ForMember(t => t.Signature, opt => opt.Ignore());

            //CreateMap<StoryRequestModel, Post>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
            //    .ForMember(t => t.BackgroundColor, opt => opt.MapFrom(x => x.BackgroundColor))
            //    .ForMember(t => t.StoryType, opt => opt.MapFrom(x => x.Category))
            //    .ForMember(t => t.OtherCategory, opt => opt.MapFrom(x => x.Category == StoryType.Other ? x.OtherCategory : null))
            //    .ForMember(t => t.Hashtags, opt => opt.Ignore())
            //    .ForMember(t => t.SpecificUsers, opt => opt.Ignore())
            //    .ForMember(t => t.MentionedUsers, opt => opt.Ignore())
            //    .ForMember(t => t.Id, opt => opt.Ignore())
            //    .ForMember(t => t.Signature, opt => opt.Ignore());

            //CreateMap<SeriesRequestModel, Series>();

            //CreateMap<Series, SeriesResponseModel>();

            //CreateMap<PostInfo, PostInfoResponseModel>()
            //    .ForMember(t => t.IsLikedByMe, opt => opt.MapFrom(x => x.Post.Likes.Any()));

            //CreateMap<DiaryInfo, PostInfoResponseModel>()
            //    .ForMember(t => t.IsLikedByMe, opt => opt.MapFrom(x => x.Diary.Likes.Any()));

            //CreateMap<PostInfo, FeedItemResponseModel>();

            //CreateMap<DiaryInfo, FeedItemResponseModel>();

            //CreateMap<MentionedUser, PostUserResponseModel>()
            //    .ForMember(t => t.Id, opt => opt.MapFrom(x => x.UserId))
            //    .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.User.UserName))
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.User.Profile.Avatar))
            //    .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.User.Profile.FirstName))
            //    .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.User.Profile.LastName))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.User.UserDeletedAt.HasValue))
            //    .ForMember(t => t.IsOnline, opt => opt.MapFrom(x => x.User.IsOnline));

            //CreateMap<SpecificUser, PostUserResponseModel>()
            //    .ForMember(t => t.Id, opt => opt.MapFrom(x => x.UserId))
            //    .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.User.UserName))
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.User.Profile.Avatar))
            //    .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.User.Profile.FirstName))
            //    .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.User.Profile.LastName))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.User.UserDeletedAt.HasValue));

            //CreateMap<FAQ, FAQResponseModel>();

            //CreateMap<ProfileInfo, ProfileInfoResponseModel>();

            //CreateMap<Profile, ProfileBaseResponseModel>()
            //    .ForMember(t => t.User, opt => opt.MapFrom(x => x));

            //CreateMap<Profile, ProfileBaseChatResponseModel>()
            //    .ForMember(t => t.User, opt => opt.MapFrom(x => x));

            //CreateMap<Post, PostBaseResponseModel>()
            //    .ForMember(t => t.Post, opt => opt.MapFrom(x => x));

            //CreateMap<Comment, CommentResponseModel>()
            //   .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //   .ForMember(t => t.RepliesCount, opt => opt.MapFrom(x => x.Comments.Count()));

            //CreateMap<PostInfo, LikeBaseResponseModel>()
            //   .ForMember(t => t.LikeResponse, opt => opt.MapFrom(x => x));

            //CreateMap<DiaryInfo, LikeBaseResponseModel>()
            //   .ForMember(t => t.LikeResponse, opt => opt.MapFrom(x => x));

            //CreateMap<PostInfo, LikeResponseModel>();

            //CreateMap<DiaryInfo, LikeResponseModel>();

            //CreateMap<Post, ListPostsBaseResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Audio, opt => opt.MapFrom(x => x.Audio))
            //    .ForMember(t => t.Category, opt => opt.MapFrom(x => x.StoryType))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info))
            //    .ForMember(t => t.MentionedUsers, opt => opt.MapFrom(x => x.MentionedUsers))
            //    .ForMember(t => t.SpecificUsers, opt => opt.MapFrom(x => x.SpecificUsers));

            //CreateMap<Advertisment, AdvertismentResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar))
            //    .ForMember(t => t.Content, opt => opt.MapFrom(x => x.Content));

            //CreateMap<AdvertismentRequestModel, Advertisment>()
            //    .ForMember(t => t.Status, opt => opt.MapFrom(x => AdvertismentStatus.Active))
            //    .ForMember(t => t.LeftDisplays, opt => opt.MapFrom(x => x.TotalDisplays))
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow));

            //CreateMap<Notification, NotificationResponseModel>()
            //   .ForMember(t => t.Creators, opt => opt.Ignore())
            //   .ForMember(t => t.Publications, opt => opt.Ignore())
            //   .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()));

            //CreateMap<Notification, NotificationFollowersResponseModel>()
            //    .IncludeBase<Notification, NotificationResponseModel>();

            //CreateMap<Post, NotificationPublicationResponseModel>()
            //    .ForMember(t => t.Type, opt => opt.MapFrom(x => x.Type))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Post));

            //CreateMap<Diary, NotificationPublicationResponseModel>()
            //    .ForMember(t => t.Type, opt => opt.MapFrom(x => x.Type))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Diary));

            //CreateMap<Comment, NotificationCommentResponseModel>();

            //CreateMap<Like, NotificationLikeInternalModel>()
            //    .ForMember(t => t.UserId, opt => opt.MapFrom(x => x.UserId))
            //    .ForMember(t => t.Date, opt => opt.MapFrom(x => x.CreatedAt.Date));

            //CreateMap<SubscriptionRequest, FollowerResponseModel>()
            //   .ForMember(t => t.Id, opt => opt.MapFrom(x => x.FromId))
            //   .ForMember(t => t.UserName, opt => opt.MapFrom(x => x.FromUser.UserName))
            //   .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.FromUser.Profile.FirstName))
            //   .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.FromUser.Profile.LastName))
            //   .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.FromUser.Profile.Avatar))
            //   .ForMember(t => t.SubscribedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //   .ForMember(t => t.Role, opt => opt.MapFrom(x => x.FromUser.UserRoles.Any() ? x.FromUser.UserRoles.First().Role : null))
            //   .ForMember(t => t.IsBlockedByMe, opt => opt.MapFrom(x => x.FromUser.ReceivedBlocks.AnyOrNull()))
            //   .ForMember(t => t.IsBlockedMe, opt => opt.MapFrom(x => x.FromUser.CreatedBlocks.AnyOrNull()))
            //   .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.FromUser.UserDeletedAt.HasValue));

            //CreateMap<Profile, TopAuthorResponseModel>()
            //    .IncludeBase<Profile, PostUserResponseModel>()
            //    //.ForMember(t => t.PublicationsCount, opt => opt.MapFrom(x => x.Info.PublicationsCount)) // UNCOMMENTING THIS LINE WILL DROP THE SERVER
            //    .ForMember(t => t.FollowersCount, opt => opt.MapFrom(x => x.Info.FollowersCount))
            //    .ForMember(t => t.IsDeleted, opt => opt.MapFrom(x => x.User.UserDeletedAt.HasValue))
            //    .ForMember(t => t.LikesCount, opt => opt.Ignore());

            //CreateMap<Signature, PostSignatureResponseModel>();

            //CreateMap<Diary, FeedItemResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.Video, opt => opt.MapFrom(x => x.Video))
            //    .ForMember(t => t.Audio, opt => opt.MapFrom(x => x.Audio))
            //    .ForMember(t => t.Info, opt => opt.MapFrom(x => x.Info))
            //    .ForMember(t => t.SpecificUsers, opt => opt.MapFrom(x => x.SpecificUsers))
            //    .ForMember(t => t.IsSaved, opt => opt.MapFrom(x => x.Favourites.Any()))
            //    .ForMember(t => t.Date, opt => opt.MapFrom(x => x.Date.ToISO()))
            //    .ForMember(t => t.Creator, opt => opt.MapFrom(x => x.User))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Diary));

            //CreateMap<Post, SearchBaseResponseModel>()
            //    .ForMember(t => t.Post, opt => opt.MapFrom(x => x));

            //CreateMap<Post, SearchResponseModel>()
            //    .IncludeBase<Post, PostResponseModel>()
            //    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
            //    .ForMember(t => t.ProfilePublicationType, opt => opt.MapFrom(x => ProfilePublicationType.Post));

            //CreateMap<MentionedUser, CommentMentoinedUserResponseModel>()
            //    .IncludeBase<MentionedUser, PostUserResponseModel>()
            //    .ForMember(t => t.MentionRange, opt => opt.MapFrom(x => x.MentionRange));

            //CreateMap<MentionRange, MentionRangeResponseModel>();

            //CreateMap<Like, TrashLikeInternalModel>()
            //    .IncludeBase<Like, NotificationLikeInternalModel>();

            //CreateMap<ApplicationUser, NotificationsSettingsResponsemodel>();
        }
    }
}
