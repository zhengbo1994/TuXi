using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.MapReleationApp.Dtos;

namespace InfoEarthFrame.Application.MapApp.Dtos
{
    public class MapInfoDto
    {
        public MapInputDto mapDto { get; set; }

        public List<MapReleationDto> listMapReleationDto { get; set; }
    }
}
