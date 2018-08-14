namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 规则响应DTO
    /// </summary>
    public class RuleStepRespDTO
    {
        public string SysCode { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public int ModelId { get; set; }
        public int VersionId { get; set; }

        //public override string ToString()
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        //}
    }
}
