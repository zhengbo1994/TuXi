using Abp.Domain.Entities;
using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InfoEarthFrame.Core
{
     [Table("TBL_SLOPE")]
    public class SlopeEntity : Entity<string>
    {
  

        [MaxLength(16)]
        public string UNIFIEDCODE { get; set; }
        [MaxLength(12)]
        public string CGHUNIFIEDCODE { get; set; }
        [MaxLength(255)]
        public string DISASTERUNITNAME { get; set; }
        [MaxLength(30)]
        public string OUTDOORCODE { get; set; }
        [MaxLength(20)]
        public string INDOORCODE { get; set; }
        [MaxLength(6)]
        public string PROVINCE { get; set; }
        [MaxLength(6)]
        public string CITY { get; set; }
        [MaxLength(9)]
        public string TOWN { get; set; }
        [MaxLength(250)]
        public string LOCATION { get; set; }
        public decimal? LATITUDE { get; set; }
        public decimal? LONGITUDE { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public decimal? Z { get; set; }
        public decimal? ALTTOP { get; set; }
        public decimal? ALTBOTOM { get; set; }

        [MaxLength(1)]
        public string SLOPETYPE { get; set; }
        [MaxLength(50)]
        public string STRATIGRAPHICTIME { get; set; }
        [MaxLength(20)]
        public string LITHOLOGY { get; set; }
        public decimal? STRATUMDIRECTION { get; set; }
        public decimal? STRATUMANGLE { get; set; }
        [MaxLength(50)]
        public string TECTONICREGION { get; set; }
        [MaxLength(1)]
        public string EARTHQUAKEINTENSITY { get; set; }
        [MaxLength(20)]
        public string MICROTOPOGRAPHY { get; set; }
        [MaxLength(20)]
        public string GROUNDWATERTYPE { get; set; }
        public decimal? AVERAGEYEARRAINFALL { get; set; }
        public decimal? MAXDAYRAINFALL { get; set; }
        public decimal? MAXHOURRAINFALL { get; set; }
        public decimal? MAXWATERLEVEL { get; set; }
        public decimal? MINWATERLEVEL { get; set; }
        [MaxLength(4)]
        public string POSITIONTORIVER { get; set; }
        [MaxLength(20)]
        public string LANDUSAGE { get; set; }
        public decimal? SLOPEHEIGHT { get; set; }
        public decimal? SLOPELENGTH { get; set; }
        public decimal? SLOPEWIDTH { get; set; }
        public decimal? SLOPEANGLE { get; set; }
        public decimal? SLOPEDIRECTION { get; set; }
        [MaxLength(1)]
        public string SLOPESHAPE { get; set; }
        [MaxLength(1)]
        public string ROCKARCHTYPE { get; set; }
        public decimal? ROCKDEPTH { get; set; }
        public decimal? FRACTUREGROUPNUM { get; set; }
        public decimal? ROCKLENGTH { get; set; }
        public decimal? ROCKWIDTH { get; set; }
        public decimal? ROCKHEIGHT { get; set; }
        [MaxLength(1)]
        public string SLOPEARCHTYPE { get; set; }
        [MaxLength(1)]
        public string SLOPEASPECTARCHTYPE { get; set; }
        [MaxLength(1)]
        public string CTRLSURFTYPE1 { get; set; }
        public decimal? CTRLSURFDIRECTION1 { get; set; }
        public decimal? CTRLSURFANGLE1 { get; set; }
        public decimal? CTRLSURFLENGTH1 { get; set; }
        public decimal? CTRLSURFSPACE1 { get; set; }
        [MaxLength(1)]
        public string CTRLSURFTYPE2 { get; set; }
        public decimal? CTRLSURFDIRECTION2 { get; set; }
        public decimal? CTRLSURFANGLE2 { get; set; }
        public decimal? COCTRLSURFLENGTH2 { get; set; }
        public decimal? CTRLSURFSPACE2 { get; set; }
        [MaxLength(1)]
        public string CTRLSURFTYPE3 { get; set; }
        public decimal? CTRLSURFDIRECTION3 { get; set; }
        public decimal? CTRLSURFANGLE3 { get; set; }
        public decimal? CTRLSURFLENGTH3 { get; set; }
        public decimal? CTRLSURFSPACE3 { get; set; }
        public decimal? WEATHEREDZONEDEPTH { get; set; }
        public decimal? UNLOADCRACKDEPTH { get; set; }
        [MaxLength(50)]
        public string SOILTEXTURENAME { get; set; }
        [MaxLength(1)]
        public string SOILDENSITYDEGREE { get; set; }
        [MaxLength(20)]
        public string SOILCONSISTENCY { get; set; }
        [MaxLength(50)]
        public string BEDROCKTIME { get; set; }
        [MaxLength(50)]
        public string BEDROCKLITHOLOGY { get; set; }
        public decimal? BEDROCKANGLE { get; set; }
        public decimal? BEDROCKDIRECTION { get; set; }
        public decimal? BEDROCKDEPTH { get; set; }
        public decimal? GROUNDWATERDEPTH { get; set; }
        [MaxLength(20)]
        public string GROUNDWATEROUTCROP { get; set; }
        [MaxLength(20)]
        public string GROUNDWATERSUPPLYTYPE { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN1 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE1 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER1 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE1 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN2 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE2 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER2 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE2 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN3 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE3 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER3 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE3 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN4 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE4 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER4 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE4 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN5 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE5 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER5 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE5 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN6 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE6 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER6 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE6 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN7 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE7 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER7 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE7 { get; set; }
        [MaxLength(1)]
        public string DISTORTIONSIGN8 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONPLACE8 { get; set; }
        [MaxLength(200)]
        public string DISTORTIONCHARACTER8 { get; set; }
        [MaxLength(50)]
        public string DISTORTIONINITDATE8 { get; set; }
        [MaxLength(30)]
        public string ASTABLEFACTOR { get; set; }
        [MaxLength(1)]
        public string CURRENTSTABLESTATUS { get; set; }
        [MaxLength(1)]
        public string STABLETREND { get; set; }
        public decimal? DESTROYEDHOME { get; set; }
        public decimal? DESTROYEDROAD { get; set; }
        public decimal? DESTROYEDCANAL { get; set; }
        [MaxLength(50)]
        public string OTHERHARM { get; set; }
        public decimal? DIRECTLOSSES { get; set; }
        [MaxLength(1)]
        public string DISASTERLEVEL { get; set; }
        public decimal? THREATENPEOPLE { get; set; }
        public decimal? THREATENASSETS { get; set; }
        [MaxLength(1)]
        public string DANGERLEVEL { get; set; }
        [MaxLength(20)]
        public string SLOPEMONITORSUGGESTION { get; set; }
        [MaxLength(50)]
        public string TREATMENTSUGGESTION { get; set; }
        [MaxLength(20)]
        public string MONITORMASS { get; set; }
        [MaxLength(18)]
        public string VILLAGEHEADER { get; set; }
        [MaxLength(20)]
        public string PHONE { get; set; }
        [MaxLength(1)]
        public string PREVENTIONPLAN { get; set; }
        [MaxLength(1)]
        public string HIDDENDANGER { get; set; }
        [MaxLength(10)]
        public string SURVEYHEADER { get; set; }
        [MaxLength(10)]
        public string FILLTABLEPEOPLE { get; set; }
        [MaxLength(10)]
        public string VERIFYPEOPLE { get; set; }
        [MaxLength(36)]
        public string CONTACTDEPTID { get; set; }
        [MaxLength(20)]
        public string FILLTABLEDATE { get; set; }
        public DateTime? UPDATETIME { get; set; }
        [MaxLength(1)]
        public string RECORDSTATUS { get; set; }
        [MaxLength(1)]
        public string EXPSTATUS { get; set; }
        [MaxLength(20)]
        public string CONTROLSTATE { get; set; }
        [MaxLength(50)]
        public string CREATORUSERID { get; set; }
        //public DateTime CREATORTIME {get;set;}
        [MaxLength(50)]
        public string UPDATEUSERID { get; set; }
        public decimal? UPDATETIMES { get; set; }
        [MaxLength(36)]
        public string PROJECTID { get; set; }
        [MaxLength(36)]
        public string MAPID { get; set; }
        [MaxLength(100)]
        public string MAPNAME { get; set; }
        [MaxLength(36)]
        public string COUNTYCODE { get; set; }
        public decimal? DESTROYEDHOUSE { get; set; }
        [MaxLength(1)]
        public string ISRSPNT { get; set; }
        [MaxLength(1)]
        public string ISSURVEYPNT { get; set; }
        [MaxLength(1)]
        public string ISMEASURINGPNT { get; set; }
        [MaxLength(500)]
        public string OUTDOORRECORD { get; set; }
        [MaxLength(50)]
        public string VILLAGE { get; set; }
        [MaxLength(50)]
        public string TEAM { get; set; }
        [MaxLength(10)]
        public string SLOPEDISTORTIONTREND { get; set; }
        public decimal? PREDICTIVEVOLUME { get; set; }
        [MaxLength(10)]
        public string PREDICTIVESCALELEVEL { get; set; }
        public decimal? SLOPEDEPTH { get; set; }
        [MaxLength(50)]
        public string THREATENOBJECT { get; set; }
        public decimal? THREATENHOME { get; set; }
        [MaxLength(50)]
        public string MASSMONITORING { get; set; }
        [MaxLength(50)]
        public string RELOCATION { get; set; }
        [MaxLength(50)]
        public string PROFESSIONMONITORING { get; set; }
        [MaxLength(50)]
        public string ENGINEERINGMANAGEMENT { get; set; }
        [MaxLength(100)]
        public string RIVERBASIN { get; set; }
        public decimal? MISSINGPERSON { get; set; }
        public decimal? INJUREDPERSON { get; set; }
        [MaxLength(1)]
        public string ISZLGCPNT { get; set; }
        [MaxLength(1)]
        public string ISMONITORPNT { get; set; }
        [MaxLength(50)]
        public string DISASTERSID { get; set; }
        [MaxLength(50)]
        public string UPDATEUSER { get; set; }
        [MaxLength(36)]
        public string MONITORMASSID { get; set; }
        [MaxLength(36)]
        public string VILLAGEHEADERID { get; set; }
        [MaxLength(36)]
        public string SURVEYHEADERID { get; set; }
        [MaxLength(36)]
        public string FILLTABLEPEOPLEID { get; set; }
        [MaxLength(36)]
        public string VERIFYPEOPLEID { get; set; }
        [MaxLength(50)]
        public string SURVEYDEPT { get; set; }

      
    }
  
}
