using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchDB;

namespace WatchJieLi.Mapper
{
    public class AutoMapperConfiguration
    {
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                #region
                //将领域实体映射到视图实体
                cfg.CreateMap<WatchGroup, LayerGroup>().ForMember(a => a.Left, b => b.MapFrom(c => c.Left))
                .ForMember(a => a.Top, b => b.MapFrom(c => c.Top));

                //cfg.CreateMap<DataFormat, DataFormatX1>();
                //cfg.CreateMap<DataFormat, DataFormatX2>();
                //cfg.CreateMap<DataFormat, DataFormatX3>();
                //cfg.CreateMap<DataFormat, DataFormatX4>();
                //cfg.CreateMap<DataFormat, DataFormatX5>();
                //cfg.CreateMap<DataFormat, DataFormatX6>();
                //cfg.CreateMap<DataFormat, DataFormatX7>();
                //cfg.CreateMap<DataFormat, DataFormatX8>();

                #endregion
            });

            Mapper = MapperConfiguration.CreateMapper();
        }

        public static IMapper? Mapper { get; private set; }

        public static MapperConfiguration? MapperConfiguration { get; private set; }
    }
}
