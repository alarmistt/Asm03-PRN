//using AutoMapper;
//using BusinessObject.Entities;
//using Services.Models.DTO;
//using Core;

//public class MappingProfile : Profile
//{
//    public MappingProfile()
//    {
//        CreateMap<Category, CategoryDTO>().ReverseMap();
//        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PaginatedListConverter<,>));
//    }
//}

//public class PaginatedListConverter<TSource, TDestination> : ITypeConverter<PaginatedList<TSource>, PaginatedList<TDestination>>
//{
//    public PaginatedList<TDestination> Convert(PaginatedList<TSource> source, PaginatedList<TDestination> destination, ResolutionContext context)
//    {
//        var mappedItems = context.Mapper.Map<IReadOnlyCollection<TDestination>>(source.Items);
//        return new PaginatedList<TDestination>(mappedItems, source.TotalCount, source.PageNumber, source.PageSize);
//    }
//}




using AutoMapper;
using BusinessObject.Entities;
using Core;
using Services.Models.DTO;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<Member, MemberDTO>().ReverseMap();
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PaginatedListConverter<,>));
    }
}

public class PaginatedListConverter<TSource, TDestination> : ITypeConverter<PaginatedList<TSource>, PaginatedList<TDestination>>
{
    public PaginatedList<TDestination> Convert(PaginatedList<TSource> source, PaginatedList<TDestination> destination, ResolutionContext context)
    {
        var mappedItems = context.Mapper.Map<IReadOnlyCollection<TDestination>>(source.Items);
        return new PaginatedList<TDestination>(mappedItems, source.TotalCount, source.PageNumber, source.PageSize);
    }
}