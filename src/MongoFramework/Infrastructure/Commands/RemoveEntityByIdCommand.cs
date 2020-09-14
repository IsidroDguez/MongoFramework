﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using MongoFramework.Infrastructure.DefinitionHelpers;
using MongoFramework.Infrastructure.Mapping;

namespace MongoFramework.Infrastructure.Commands
{
	public class RemoveEntityByIdCommand<TEntity> : IWriteCommand<TEntity> where TEntity : class
	{
		private object EntityId { get; }
		private string TenantId { get; }

		public Type EntityType => typeof(TEntity);

		public RemoveEntityByIdCommand(object entityId, string tenantId = null)
		{
			EntityId = entityId;
			TenantId = tenantId;
		}

		public IEnumerable<WriteModel<TEntity>> GetModel()
		{
			var definition = EntityMapping.GetOrCreateDefinition(typeof(TEntity));
			yield return new DeleteOneModel<TEntity>(definition.CreateIdFilter<TEntity>(EntityId, TenantId));
		}
	}
}
