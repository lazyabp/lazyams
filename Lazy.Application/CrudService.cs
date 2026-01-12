using AutoMapper.Internal.Mappers;
using Lazy.Application.Contracts;
using Lazy.Core.Security;
using Lazy.Core.Utils;

namespace Lazy.Application;

public abstract class CrudService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput> :
      ReadOnlyService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>,
      ICrudService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
      where TEntity : BaseEntity
{

    public ICurrentUser CurrentUser { get; set; }

    public CrudService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {

    }

    public virtual async Task<TGetOutputDto> CreateAsync(TCreateInput input)
    {
        var entity = MapToEntity(input);
        GetDbSet().Add(entity);
        await this.LazyDBContext.SaveChangesAsync();
        return MapToGetOutputDto(entity);
    }

    /// <summary>
    /// Maps <typeparamref name="TCreateInput"/> to <typeparamref name="TEntity"/> to create a new entity.
    /// It uses <see cref="IObjectMapper"/> by default.
    /// It can be overriden for custom mapping.
    /// </summary>
    protected virtual TEntity MapToEntity(TCreateInput createInput)
    {
        var entity = this.Mapper.Map<TCreateInput, TEntity>(createInput);
        SetIdForLong(entity);
        SetCreatedAudit(entity);
        return entity;
    }

    /// <summary>
    /// Sets Id value for the entity if <typeparamref name="TKey"/> is <see cref="long"/>.
    /// It's used while creating a new entity.
    /// </summary>
    protected virtual void SetIdForLong(TEntity entity)
    {
        if (entity is BaseEntity baseEntity && baseEntity.Id == 0)
        {
            baseEntity.Id = SnowflakeIdGeneratorUtil.NextId();
        }
    }

    /// <summary>
    /// Set created datetime & created user
    /// </summary>
    /// <param name="entity">added object</param>
    protected virtual void SetCreatedAudit(TEntity entity)
    {
        if (entity is BaseEntityWithAudit baseEntityWithAudit && !baseEntityWithAudit.CreatedBy.HasValue)
        {
            baseEntityWithAudit.CreatedBy = CurrentUser.Id;
            baseEntityWithAudit.CreatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Set updated datetime & updated user
    /// </summary>
    /// <param name="entity">added object</param>
    protected virtual void SetUpdatedAudit(TEntity entity)
    {
        if (entity is BaseEntityWithAudit baseEntityWithAudit && !baseEntityWithAudit.UpdatedBy.HasValue)
        {
            baseEntityWithAudit.UpdatedBy = CurrentUser.Id;
            baseEntityWithAudit.UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Set deleted datetime & deleted user
    /// </summary>
    /// <param name="entity">delete object</param>
    protected virtual void SetDeletedAudit(TEntity entity)
    {
        if (entity is BaseEntityWithSoftDelete baseEntityWithSoftDelete && !baseEntityWithSoftDelete.DeletedBy.HasValue)
        {
            baseEntityWithSoftDelete.IsDeleted = true;
            baseEntityWithSoftDelete.DeletedBy = CurrentUser.Id;
            baseEntityWithSoftDelete.UpdatedAt = DateTime.Now;
        }
    }


    /// <summary>
    /// Sets Id value for the entity if <typeparamref name="TKey"/> is <see cref="long"/>.
    /// It's used while creating a new entity.
    /// </summary>
    protected virtual void SetIdForLong(BaseEntity baseEntity)
    {
        baseEntity.Id = SnowflakeIdGeneratorUtil.NextId();
    }

    /// <summary>
    /// Maps <typeparamref name="TUpdateInput"/> to <typeparamref name="TEntity"/> to update the entity.
    /// It uses <see cref="IMapper"/> by default.
    /// It can be overriden for custom mapping.
    /// </summary>
    protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
    {
        this.Mapper.Map(updateInput, entity);
        SetUpdatedAudit(entity);
    }

    public virtual async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
    {
        var entity = await GetEntityByIdAsync(id);

        MapToEntity(input, entity);
       
        var dbSet = GetDbSet();
        if (dbSet.Local.All(e => e != entity))
        {
            dbSet.Attach(entity);
            dbSet.Update(entity);
        }

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public virtual async Task DeleteAsync(TKey id)
    {
        var entity = await GetEntityByIdAsync(id);
        var dbSet = GetDbSet();
        dbSet.Remove(entity);
        await this.LazyDBContext.SaveChangesAsync();
    }

    public virtual async Task SoftDeleteAsync(TKey id)
    {
        var entity = await GetEntityByIdAsync(id);
        SetDeletedAudit(entity);

        var dbSet = GetDbSet();
        dbSet.Update(entity);

        await this.LazyDBContext.SaveChangesAsync();
    }
}

