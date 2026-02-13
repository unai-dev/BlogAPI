using AutoMapper;
using BlogAPI.DTOs.Post;
using BlogAPI.DTOs.User;
using BlogAPI.Entities;

namespace BlogAPI.Utils
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Post, PostDTO>().ForMember(dto => dto.UserEmail, config => config.MapFrom(ent => ent.User!.Email));
            CreateMap<AddPostDTO, Post>();

            CreateMap<User, UserDTO>();
        }
    }
}
