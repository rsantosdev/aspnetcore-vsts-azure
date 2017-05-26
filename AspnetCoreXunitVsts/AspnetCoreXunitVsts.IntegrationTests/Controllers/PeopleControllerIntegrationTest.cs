﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspnetCoreXunitVsts.Api.Models;
using AspnetCoreXunitVsts.IntegrationTests.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace AspnetCoreXunitVsts.IntegrationTests.Controllers
{
    public class PeopleControllerIntegrationTest : BaseIntegrationTest
    {
        private const string BaseUrl = "/api/people";

        public PeopleControllerIntegrationTest(BaseTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetShouldReturnValues()
        {
            var person = await CreatePerson();

            var response = await Client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            var dataString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<Person>>(dataString);

            Assert.Equal(data.Count, 1);
            Assert.Contains(data, x => x.Name == person.Name);
        }

        [Fact]
        public async Task GetSinglePersonNoIdReturnsNotFound()
        {
            var response = await Client.GetAsync($"{BaseUrl}/99");
            Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetSinglePersonWithIdReturnsPerson()
        {
            var person = await CreatePerson();

            var response = await Client.GetAsync($"{BaseUrl}/{person.Id}");
            response.EnsureSuccessStatusCode();

            var dataString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Person>(dataString);

            Assert.Equal(data.Name, person.Name);
        }

        [Fact]
        public async Task PostWithInvalidModelShouldReturnBadRequest()
        {
            var body = new { value = "my-value" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(BaseUrl, content);

            Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostWithValidModelShouldReturnCreated()
        {
            var person = new Person
            {
                Name = "Rafael dos Santos",
                Phone = "+5527900000000",
                BirthDay = new DateTime(1988, 09, 08),
                Salary = 1000
            };

            var content = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();

            var data = JsonConvert.DeserializeObject<Person>(await response.Content.ReadAsStringAsync());

            Assert.Equal(response.StatusCode, HttpStatusCode.Created);
            Assert.NotSame(data.Id, 0);
            Assert.Equal(data.Name, person.Name);
        }

        [Fact]
        public async Task UpdatePersonShouldReturnNotFound()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await Client.PutAsync($"{BaseUrl}/999", content);

            Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
        }

        private async Task<Person> CreatePerson()
        {
            var person = new Person
            {
                Name = "Rafael dos Santos",
                Phone = "+5527900000000",
                BirthDay = new DateTime(1988, 09, 08),
                Salary = 1000
            };

            await TestDbContext.AddAsync(person);
            await TestDbContext.SaveChangesAsync();
            return person;
        }
    }
}
