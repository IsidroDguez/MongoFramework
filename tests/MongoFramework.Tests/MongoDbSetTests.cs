﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MongoFramework.Tests
{
	[TestClass]
	public class MongoDbSetTests : TestBase
	{
		public class TestModel
		{
			public string Id { get; set; }

			public string Description { get; set; }
			public bool BooleanField { get; set; }
		}

		[TestMethod]
		public void SuccessfulInsertAndQueryBack()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			dbSet.Add(new TestModel
			{
				Description = "ValueSync"
			});

			Assert.IsFalse(dbSet.Any(m => m.Description == "ValueSync"));
			context.SaveChanges();
			Assert.IsTrue(dbSet.Any(m => m.Description == "ValueSync"));
		}

		[TestMethod]
		public async Task SuccessfulInsertAndQueryBackAsync()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			dbSet.Add(new TestModel
			{
				Description = "ValueAsync"
			});

			Assert.IsFalse(dbSet.Any(m => m.Description == "ValueAsync"));
			await context.SaveChangesAsync();
			Assert.IsTrue(dbSet.Any(m => m.Description == "ValueAsync"));
		}

		[TestMethod]
		public void SuccessfulInsertAndFind()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var model = new TestModel
			{
				Description = "ValueSync"
			};

			dbSet.Add(model);

			context.SaveChanges();

			context = new MongoDbContext(connection);
			dbSet = new MongoDbSet<TestModel>(context);
			Assert.IsTrue(dbSet.Find(model.Id).Description == "ValueSync");
			Assert.IsTrue(context.ChangeTracker.GetEntry(model).State == MongoFramework.Infrastructure.EntityEntryState.NoChanges);			
		}

		[TestMethod]
		public void SuccessfulNullFind()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var model = new TestModel
			{
				Description = "ValueSync"
			};

			dbSet.Add(model);

			context.SaveChanges();

			Assert.IsNull(dbSet.Find("abcd"));
		}

		[TestMethod]
		public void SuccessfullyFindTracked()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var model = new TestModel
			{
				Id = "abcd",
				Description = "ValueSync"
			};

			dbSet.Add(model);

			//Note: not saving, but still should be found as tracked
			Assert.IsTrue(dbSet.Find(model.Id).Description == "ValueSync");
			Assert.IsTrue(context.ChangeTracker.GetEntry(model).State == MongoFramework.Infrastructure.EntityEntryState.Added);
		}

		[TestMethod]
		public void SuccessfullyUpdateEntity()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entity = new TestModel
			{
				Description = "SuccessfullyUpdateEntity"
			};

			dbSet.Add(entity);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			entity.Description = "SuccessfullyUpdateEntity-Updated";
			dbSet.Update(entity);

			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyUpdateEntity-Updated"));
			context.SaveChanges();
			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyUpdateEntity-Updated"));
		}

		[TestMethod]
		public void SuccessfullyUpdateRange()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entities = new[] {
				new TestModel
				{
					Description = "SuccessfullyUpdateRange.1"
				},
				new TestModel
				{
					Description = "SuccessfullyUpdateRange.2"
				}
			};

			dbSet.AddRange(entities);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			entities[0].Description = "SuccessfullyUpdateRange.1-Updated";
			entities[1].Description = "SuccessfullyUpdateRange.2-Updated";
			dbSet.UpdateRange(entities);

			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyUpdateRange.1-Updated" || m.Description == "SuccessfullyUpdateRange.2-Updated"));
			context.SaveChanges();
			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyUpdateRange.1-Updated"));
			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyUpdateRange.2-Updated"));
		}

		[TestMethod]
		public void SuccessfullyRemoveEntity()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entity = new TestModel
			{
				Description = "SuccessfullyRemoveEntity"
			};

			dbSet.Add(entity);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			dbSet.Remove(entity);

			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyRemoveEntity"));
			context.SaveChanges();
			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyRemoveEntity"));
		}

		[TestMethod]
		public void SuccessfullyRemoveRange()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entities = new[] {
				new TestModel
				{
					Description = "SuccessfullyRemoveRange.1"
				},
				new TestModel
				{
					Description = "SuccessfullyRemoveRange.2"
				}
			};

			dbSet.AddRange(entities);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			dbSet.RemoveRange(entities);

			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyRemoveRange.1"));
			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyRemoveRange.2"));
			context.SaveChanges();
			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyRemoveRange.1"));
			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyRemoveRange.2"));
		}
		
		[TestMethod]
		public void SuccessfullyRemoveEntityById()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entity = new TestModel
			{
				Description = "SuccessfullyRemoveEntityById"
			};

			dbSet.Add(entity);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			dbSet.RemoveById(entity.Id);

			Assert.IsTrue(dbSet.Any(m => m.Description == "SuccessfullyRemoveEntityById"));
			context.SaveChanges();
			Assert.IsFalse(dbSet.Any(m => m.Description == "SuccessfullyRemoveEntityById"));
		}

		[TestMethod]
		public void SuccessfullyRemoveRangeByPredicate()
		{
			var connection = TestConfiguration.GetConnection();
			var context = new MongoDbContext(connection);
			var dbSet = new MongoDbSet<TestModel>(context);

			var entities = new[]
			{
				new TestModel
				{
					Description = "SuccessfullyRemoveRangeByPredicate"
				},
				new TestModel
				{
					Description = "SuccessfullyRemoveRangeByPredicate",
					BooleanField = true
				}
			};

			dbSet.AddRange(entities);
			context.SaveChanges();

			dbSet = new MongoDbSet<TestModel>(context);

			dbSet.RemoveRange(e => e.BooleanField);

			Assert.AreEqual(2, dbSet.Count(m => m.Description == "SuccessfullyRemoveRangeByPredicate"));
			context.SaveChanges();
			Assert.AreEqual(1, dbSet.Count(m => m.Description == "SuccessfullyRemoveRangeByPredicate"));
			Assert.IsNotNull(dbSet.FirstOrDefault(m => m.Id == entities[0].Id));
		}
	}
}