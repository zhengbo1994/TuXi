using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using InfoEarthFrame.Core;
using InfoEarthFrame.Application;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Application.DataTagApp.Dtos;
using InfoEarthFrame.Application.DataTypeApp.Dtos;
using InfoEarthFrame.Application.DicDataCodeApp.Dtos;
using InfoEarthFrame.Application.DicDataTypeApp.Dtos;
using InfoEarthFrame.Application.LayerFieldApp.Dtos;
using InfoEarthFrame.Application.MapApp.Dtos;
using InfoEarthFrame.Application.MapMetaDataApp.Dtos;
using InfoEarthFrame.Application.MapReleationApp.Dtos;
using InfoEarthFrame.Application.TagReleationApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Application.LayerFieldDictApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.DataManage.DTO;
using InfoEarthFrame.ShpFileReadLogApp.Dtos;

namespace InfoEarthFrame
{
    public static class DtoMappings
    {
        public static void Map()
        {
            //I specified mapping for AssignedPersonId since NHibernate does not fill Task.AssignedPersonId
            //If you will just use EF, then you can remove ForMember definition.
            //Mapper.CreateMap<Task, TaskDto>().ForMember(t => t.AssignedPersonId, opts => opts.MapFrom(d => d.AssignedPerson.Id));
            Mapper.CreateMap<AttachmentEntity, AttachmentDto>();
            Mapper.CreateMap<HazardsTypeEntity, HazardsTypeDto>();
            Mapper.CreateMap<DisasterEntity, DisasterDto>();
            Mapper.CreateMap<Tbl_LayerManager, LayerManagerDto>();
            Mapper.CreateMap<MultimediaTypeEntity, MultimediaTypeDto>();


            Mapper.CreateMap<DataStyleEntity, DataStyleDto>();
            Mapper.CreateMap<DataStyleDto, DataStyleEntity>();

            Mapper.CreateMap<DataTagEntity, DataTagDto>();
            Mapper.CreateMap<DataTagDto, DataTagEntity>();

            Mapper.CreateMap<DataTypeEntity, DataTypeDto>();
            Mapper.CreateMap<DataTypeDto, DataTypeEntity>();

            Mapper.CreateMap<DicDataCodeEntity, DicDataCodeDto>();
            Mapper.CreateMap<DicDataCodeDto, DicDataCodeEntity>();

            Mapper.CreateMap<DicDataTypeEntity, DicDataTypeDto>();
            Mapper.CreateMap<DicDataTypeDto, DicDataTypeEntity>();

            Mapper.CreateMap<LayerContentEntity, LayerContentDto>();
            Mapper.CreateMap<LayerContentDto, LayerContentEntity>();

            Mapper.CreateMap<LayerFieldEntity, LayerFieldDto>();
            Mapper.CreateMap<LayerFieldDto, LayerFieldEntity>();

            Mapper.CreateMap<MapEntity, MapDto>();
            Mapper.CreateMap<MapDto, MapEntity>();

            Mapper.CreateMap<MapMetaDataEntity, MapMetaDataDto>();
            Mapper.CreateMap<MapMetaDataDto, MapMetaDataEntity>();

            Mapper.CreateMap<MapReleationEntity, MapReleationDto>();
            Mapper.CreateMap<MapReleationDto, MapReleationEntity>();

            Mapper.CreateMap<TagReleationEntity, TagReleationDto>();
            Mapper.CreateMap<TagReleationDto, TagReleationEntity>();

            Mapper.CreateMap<MapEntity, MapOutputDto>();
            Mapper.CreateMap<LayerContentEntity, LayerContentOutputDto>();


            Mapper.CreateMap<SystemUserEntity, SystemUserDto>();
            Mapper.CreateMap<SystemUserEntity, SystemUserOutputDto>();
            Mapper.CreateMap<SystemUserOutputDto,SystemUserEntity>();

            Mapper.CreateMap<LayerFieldDictEntity, LayerFieldDictDto>();

            Mapper.CreateMap<DataStyleEntity, DataStyleOutputDto>();

            Mapper.CreateMap<OperateLogEntity, OperateLogOutputDto>();
            Mapper.CreateMap<OperateLogOutputDto, OperateLogEntity>();

            Mapper.CreateMap<GeologyMappingType, GeologyMappingTypeDto>();
            Mapper.CreateMap<GeologyMappingTypeDto, GeologyMappingType>();


            Mapper.CreateMap<DataMain, DataMainDto>();
            Mapper.CreateMap<DataMainDto, DataMain>();

            Mapper.CreateMap<UploadFileDto, DataManageFile>();

            Mapper.CreateMap<UploadLayerDto, LayerContentEntity>();

            Mapper.CreateMap<Layer, LayerListOutput>();


            Mapper.CreateMap<ShpFileReadLogOutputDto, ShpFileReadLogEntity>();
            Mapper.CreateMap<ShpFileReadLogEntity, ShpFileReadLogOutputDto>();

            Mapper.CreateMap<GroupEntity, GroupDto>();
            Mapper.CreateMap<GroupDto, GroupEntity>();
        }
    }
}
