using AutoMapper;
using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Dtos.Job;
using Invert.Api.Dtos.Project;
using Invert.Api.Dtos.User;
using Invert.Api.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Invert.Api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ========== Project Mappings ==========

            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Project, ProjectDto>();

            CreateMap<UpdateProjectDto, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========== Job Mappings ==========

            CreateMap<CreateJobDto, Job>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.EmploymentType.ToString()))
                .ForMember(dest => dest.ExperienceLevel, opt => opt.MapFrom(src => src.ExperienceLevel.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => src.Requirements))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.Benefits));

            CreateMap<Job, JobDto>()
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => src.Requirements))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.Benefits));

            var map = CreateMap<UpdateJobDto, Job>();

            map.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            map.ForMember(dest => dest.EmploymentType, opt =>
            {
                opt.PreCondition(src => src.EmploymentType != null);
                opt.MapFrom(src => src.EmploymentType!.ToString());
            });

            map.ForMember(dest => dest.ExperienceLevel, opt =>
            {
                opt.PreCondition(src => src.ExperienceLevel != null);
                opt.MapFrom(src => src.ExperienceLevel!.ToString());
            });

            map.ForMember(dest => dest.Status, opt =>
            {
                opt.PreCondition(src => src.Status != null);
                opt.MapFrom(src => src.Status!.ToString());
            });
            map.ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => src.Requirements));
            map.ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills));
            map.ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.Benefits));
            // var profileDto = _mapper.Map<UserProfileDto>(user);

            // ========== User Mappings ==========
            // ========== User Mappings ==========
            CreateMap<AppUser, UserProfileDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.TotalArticles, opt => opt.MapFrom(src => src.Articles.Count))
                .ForMember(dest => dest.TotalProjects, opt => opt.MapFrom(src => src.Projects.Count))
                .ForMember(dest => dest.TotalNotifications, opt => opt.MapFrom(src => src.Notifications.Count))
                .ForMember(dest => dest.UnreadNotifications, opt => opt.MapFrom(src => src.Notifications.Count(n => !n.IsRead)))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLoginAt));

            CreateMap<UpdateProfileDto, AppUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.Articles, opt => opt.Ignore())
                .ForMember(dest => dest.Projects, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========== Article Mappings ==========
            CreateMap<Article, ArticleDto>();
            CreateMap<CreateArticleDto, Article>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());





        }
    }
}