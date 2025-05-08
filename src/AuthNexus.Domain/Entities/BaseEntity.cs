namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid? CreatedBy { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }
        
        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public Guid? LastModifiedBy { get; set; }
        
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}