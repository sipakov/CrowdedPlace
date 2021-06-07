using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.MobileApi.Implementations;

namespace OnlineDemonstrator.MobileApi.Tests
{
    [TestFixture]
    public class PosterControllerTest
    {
        [Test]
        public void Find_target_posters()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters1")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var demonstration1 = new Demonstration
            {
                Id = 1,
                DemonstrationDate = DateTime.UtcNow.Date.AddDays(-1)
            };
            var demonstration2 = new Demonstration
            {
                Id = 2,
                DemonstrationDate = DateTime.UtcNow.Date
            };
            var demonstration3 = new Demonstration
            {
                Id = 3,
                DemonstrationDate = DateTime.UtcNow.Date
            };
            context.Demonstrations.Add(demonstration1);
            context.Demonstrations.Add(demonstration2);
            context.Demonstrations.Add(demonstration3);
            context.SaveChanges();
            var device1Id = Guid.NewGuid().ToString();
            var device2Id = Guid.NewGuid().ToString();
            var device3Id = Guid.NewGuid().ToString();
            var device4Id = Guid.NewGuid().ToString();
            var device5Id = Guid.NewGuid().ToString();
            var device6Id = Guid.NewGuid().ToString();
            var device7Id = Guid.NewGuid().ToString();
            var device8Id = Guid.NewGuid().ToString();
            var device9Id = Guid.NewGuid().ToString();
            var listDevices = new List<Device>();
            var device1 = new Device
            {
                Id = device1Id
            };
            listDevices.Add(device1);

            var device2 = new Device
            {
                Id = device2Id
            };
            listDevices.Add(device2);
            var device3 = new Device
            {
                Id = device3Id
            };
            listDevices.Add(device3);
            var device4 = new Device
            {
                Id = device4Id
            };
            listDevices.Add(device4);
            var device5 = new Device
            {
                Id = device5Id
            };
            listDevices.Add(device5);
            var device6 = new Device
            {
                Id = device6Id
            };
            listDevices.Add(device6);
            var device7 = new Device
            {
                Id = device7Id
            };
            listDevices.Add(device7);
            var device8 = new Device
            {
                Id = device8Id
            };
            listDevices.Add(device8);
            var device9 = new Device
            {
                Id = device9Id
            };
            listDevices.Add(device9);
            context.AddRange(listDevices);
            context.SaveChanges();
            context.Posters.Add(new Poster
            {
                DeviceId = device1Id,
                Device = device1,
                DemonstrationId = 1,
                Demonstration = demonstration1,
                CreatedDate = DateTime.UtcNow.AddHours(-24)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device2Id,
                Device = device2,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-2)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device3Id,
                Device = device3,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-3)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device4Id,
                Device = device4,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-1)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device5Id,
                Device = device5,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-4)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device6Id,
                Device = device6,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-5)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device7Id,
                Device = device7,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-1)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device8Id,
                Device = device8,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-5)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device9Id,
                Device = device9,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-3)
            });
            context.SaveChanges();

            var appContext = new ApplicationContextFactory(options);

            var service = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result = service.GetFromActualDemonstrations(2).Result;
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(result.First(x=>x.DeviceId == device4Id).DeviceId, device4Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device2Id).DeviceId, device2Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device7Id).DeviceId, device7Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device9Id).DeviceId, device9Id);
        }

        [Test]
        public void Add_poster_if_actual_demonstration_is_not_exist()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters2")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var device1Id = Guid.NewGuid().ToString();
            var poster = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.909674,
                Longitude = 12.487997,
                Name = "1",
                Title = "11",
                Message = "111"
            };
            var appContext = new ApplicationContextFactory(options);

            var currentDateTime = DateTime.UtcNow;
            var service = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result = service.AddPosterAsync(poster, currentDateTime).Result;

            var postersCountInDb = context.Posters.Count();
            Assert.AreEqual(1, postersCountInDb);
            Assert.AreEqual(device1Id, result.DeviceId);
            Assert.AreEqual(1, result.DemonstrationId);
            Assert.AreEqual(currentDateTime, result.CreatedDateTime);
            Assert.AreEqual(currentDateTime.Date, result.CreatedDate);

        }
        [Test]
        public void Add_poster_if_actual_demonstration_is_exist_and_not_far_away()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters3")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var appContext = new ApplicationContextFactory(options);

            var device1Id = Guid.NewGuid();
            var poster1 = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.909674,
                Longitude = 12.487997,
                Name = "1",
                Title = "11",
                Message = "111"
            };
            
            var service1 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result1 = service1.AddPosterAsync(poster1, DateTime.UtcNow).Result;

            var device2Id = Guid.NewGuid();
            var poster2 = new PosterIn
            {
                DeviceId = device2Id,
                Latitude = 41.908647,
                Longitude = 12.48513,
                Name = "2",
                Title = "22",
                Message = "222"
            };

            var service2 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());

            var currentDateTime = DateTime.UtcNow;
            var result2 = service2.AddPosterAsync(poster2, currentDateTime).Result;

            var posterList = context.Posters.ToList();
            
            var postersCountInDb = context.Posters.Count();
            Assert.AreEqual(2, postersCountInDb);
            
            var demonstrationsCountInDb = context.Demonstrations.Count();
            Assert.AreEqual(1, demonstrationsCountInDb);
            
            Assert.AreEqual(device2Id, result2.DeviceId);
            Assert.AreEqual(1, result1.DemonstrationId);
            Assert.AreEqual(1, result2.DemonstrationId);
            Assert.AreEqual(currentDateTime, result2.CreatedDateTime);
        }
        
        [Test]
        public void Add_poster_if_actual_demonstration_is_exist_and_far_away()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters4")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var appContext = new ApplicationContextFactory(options);

            var device1Id = Guid.NewGuid();
            var poster1 = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.909674,
                Longitude = 12.487997,
                Name = "1",
                Title = "11",
                Message = "111"
            };
            
            var service1 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result1 = service1.AddPosterAsync(poster1, DateTime.UtcNow).Result;

            var device2Id = Guid.NewGuid();
            var poster2 = new PosterIn
            {
                DeviceId = device2Id,
                Latitude = 41.857202,
                Longitude = 12.4932116,
                Name = "2",
                Title = "22",
                Message = "222"
            };

            var service2 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());

            var currentDateTime = DateTime.UtcNow;
            var result2 = service2.AddPosterAsync(poster2, currentDateTime).Result;

            var posterList = context.Posters.ToList();
            var demonstrationList = context.Demonstrations.ToList();
            
            var postersCountInDb = context.Posters.Count();
            Assert.AreEqual(2, postersCountInDb);
            
            var demonstrationsCountInDb = context.Demonstrations.Count();
            Assert.AreEqual(2, demonstrationsCountInDb);
            
            Assert.AreEqual(device2Id, result2.DeviceId);
            Assert.AreEqual(1, result1.DemonstrationId);
            Assert.AreEqual(2, result2.DemonstrationId);
            Assert.AreEqual(currentDateTime, result2.CreatedDateTime);
        }
        
        [Test]
        public void Add_poster_if_actual_poster_is_exist()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters5")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var appContext = new ApplicationContextFactory(options);

            var device1Id = Guid.NewGuid();
            var poster1 = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.909674,
                Longitude = 12.487997,
                Name = "1",
                Title = "11",
                Message = "111"
            };
            
            var service1 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result1 = service1.AddPosterAsync(poster1, DateTime.UtcNow).Result;

            var poster2 = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.857202,
                Longitude = 12.4932116,
                Name = "2",
                Title = "22",
                Message = "222"
            };

            var service2 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());

            var currentDateTime = DateTime.UtcNow;
          //  var result2 = service2.AddPosterAsync(poster2, currentDateTime).Result;

            Assert.ThrowsAsync<ArgumentException>(async () => await service2.AddPosterAsync(poster2, currentDateTime));
        }
        
        [Test]
        public void Get_posters_dy_demonstration_id()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters6")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var demonstration1 = new Demonstration
            {
                Id = 1,
                DemonstrationDate = DateTime.UtcNow.Date.AddDays(-1)
            };
            var demonstration2 = new Demonstration
            {
                Id = 2,
                DemonstrationDate = DateTime.UtcNow.Date
            };
            var demonstration3 = new Demonstration
            {
                Id = 3,
                DemonstrationDate = DateTime.UtcNow.Date
            };
            context.Demonstrations.Add(demonstration1);
            context.Demonstrations.Add(demonstration2);
            context.Demonstrations.Add(demonstration3);
            context.SaveChanges();
            var device1Id = Guid.NewGuid();
            var device2Id = Guid.NewGuid();
            var device3Id = Guid.NewGuid();
            var device4Id = Guid.NewGuid();
            var device5Id = Guid.NewGuid();
            var device6Id = Guid.NewGuid();
            var device7Id = Guid.NewGuid();
            var device8Id = Guid.NewGuid();
            var device9Id = Guid.NewGuid();
            var listDevices = new List<Device>();
            var device1 = new Device
            {
                Id = device1Id
            };
            listDevices.Add(device1);

            var device2 = new Device
            {
                Id = device2Id
            };
            listDevices.Add(device2);
            var device3 = new Device
            {
                Id = device3Id
            };
            listDevices.Add(device3);
            var device4 = new Device
            {
                Id = device4Id
            };
            listDevices.Add(device4);
            var device5 = new Device
            {
                Id = device5Id
            };
            listDevices.Add(device5);
            var device6 = new Device
            {
                Id = device6Id
            };
            listDevices.Add(device6);
            var device7 = new Device
            {
                Id = device7Id
            };
            listDevices.Add(device7);
            var device8 = new Device
            {
                Id = device8Id
            };
            listDevices.Add(device8);
            var device9 = new Device
            {
                Id = device9Id
            };
            listDevices.Add(device9);
            context.AddRange(listDevices);
            context.SaveChanges();
            context.Posters.Add(new Poster
            {
                DeviceId = device1Id,
                Device = device1,
                DemonstrationId = 1,
                Demonstration = demonstration1,
                CreatedDate = DateTime.UtcNow.AddHours(-24)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device2Id,
                Device = device2,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-2)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device3Id,
                Device = device3,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-3)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device4Id,
                Device = device4,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-1)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device5Id,
                Device = device5,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-4)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device6Id,
                Device = device6,
                DemonstrationId = 2,
                Demonstration = demonstration2,
                CreatedDate = DateTime.UtcNow.AddHours(-5)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device7Id,
                Device = device7,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-1)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device8Id,
                Device = device8,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-5)
            });
            context.Posters.Add(new Poster
            {
                DeviceId = device9Id,
                Device = device9,
                DemonstrationId = 3,
                Demonstration = demonstration3,
                CreatedDate = DateTime.UtcNow.AddHours(-3)
            });
            context.SaveChanges();

            var appContext = new ApplicationContextFactory(options);

            var service = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result = service.GetPostersByDemonstrationId(2).Result;
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(result.First(x=>x.DeviceId == device2Id).DeviceId, device2Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device3Id).DeviceId, device3Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device4Id).DeviceId, device4Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device5Id).DeviceId, device5Id);
            Assert.AreEqual(result.First(x=>x.DeviceId == device6Id).DeviceId, device6Id);
        }
        
        [Test]
        public void Get_poster_by_id()
        {
             var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "Posters7")
                .Options;
            var context = new ApplicationContextFactory(options).CreateContext();
            
            var appContext = new ApplicationContextFactory(options);

            var device1Id = Guid.NewGuid();
            var poster1 = new PosterIn
            {
                DeviceId = device1Id,
                Latitude = 41.909674,
                Longitude = 12.487997,
                Name = "1",
                Title = "11",
                Message = "111"
            };
            
            var service1 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());
            var result1 = service1.AddPosterAsync(poster1, DateTime.UtcNow).Result;

            var device2Id = Guid.NewGuid();
            var poster2 = new PosterIn
            {
                DeviceId = device2Id,
                Latitude = 41.908647,
                Longitude = 12.48513,
                Name = "2",
                Title = "22",
                Message = "222"
            };

            var service2 = new PosterService(appContext, new DemonstrationService(appContext), new DistanceCalculator());

            var currentDateTime = DateTime.UtcNow;
            var result2 = service2.AddPosterAsync(poster2, currentDateTime).Result;

            var posterList = context.Posters.ToList();
            var demonstrationList = context.Demonstrations.ToList();
            
            var postersCountInDb = context.Posters.Count();
            Assert.AreEqual(2, postersCountInDb);
            
            var demonstrationsCountInDb = context.Demonstrations.Count();
            Assert.AreEqual(1, demonstrationsCountInDb);
            
            Assert.AreEqual(device1Id, result1.DeviceId);
            Assert.AreEqual(device2Id, result2.DeviceId);
            Assert.AreEqual(1, result1.DemonstrationId);
            Assert.AreEqual(1, result2.DemonstrationId);
        }
    }
}