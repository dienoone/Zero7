using AutoMapper;
using O7.Core.Models.O7Models.Main;
using O7.Core.ViewModels.O7ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Color Controller:
            CreateMap<Color, ColorDto>().ReverseMap();

            // Size Controller:
            CreateMap<Size, SizeDto>().ReverseMap();

            // Style controller:
            CreateMap<Style, StyleDto>().ReverseMap();

            // Type Controller:
            CreateMap<ProductType, TypeDto>().ReverseMap();

            // Gender Controller : 
            CreateMap<Gender, GenderDto>().ReverseMap();

            // Season Controller : 
            CreateMap<Season, SeasonDto>().ReverseMap();

            // Collection Controller:
            CreateMap<Collection, CollectionDto>()
                .ForMember(dest => dest.CollectionPhotos, src => src.MapFrom(e => e.CollectionPhotos.Select(n => new CollectionPhotosDto 
                { 
                    Id = n.Id,
                    IsActive = n.IsActive,
                    IsFavorite = n.IsFavorite,
                    Photo = n.Photo
                })));

            CreateMap<Product, ProductDto>()
                .ForMember(e => e.TypeName, src => src.MapFrom(src => src.ProductType.Name))
                .ForMember(e => e.TypeId, src => src.MapFrom(src => src.ProductType.Id))
                .ForMember(e => e.StyleName, src => src.MapFrom(src => src.Style.Name))
                .ForMember(e => e.Colors, src => src.MapFrom(src => src.ProductColors));

            CreateMap<ProductColor, ProductColorDto>()
                .ForMember(e => e.ColorName, src => src.MapFrom(e => e.Color.Name))
                .ForMember(e => e.ColorCode, src => src.MapFrom(e => e.Color.Code))
                .ForMember(e => e.Photos, src => src.MapFrom(e => e.ProductColorImages))
                .ForMember(e => e.Sizes, src => src.MapFrom(e => e.ProductColorSizes));

            CreateMap<ProductColorImage, ProductColorImageDto>();
            CreateMap<ProductColorSize, ProductColorSizeDto>()
                .ForMember(e => e.SizeName, src => src.MapFrom(e => e.Size.Name));


        }
    }
}
