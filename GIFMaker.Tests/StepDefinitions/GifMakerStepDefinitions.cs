using GIFMaker.Contracts;
using GIFMaker.Entities;
using GIFMaker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TechTalk.SpecFlow.Assist;

namespace GIFMaker.Tests.StepDefinitions
{
    [Binding]
    public class GifMakerStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        public GifMakerStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario] 
        public void Setup()
        {
            if (Directory.Exists(CustomWebApplicationFactory<Program>.FolderPath))
            {
                Directory.Delete(CustomWebApplicationFactory<Program>.FolderPath, true); 
            }

            var webAppFactory = new CustomWebApplicationFactory<Program>();
            var client = webAppFactory.CreateClient();
            using var scope = webAppFactory.Services.CreateScope();
            var gifRepository = scope.ServiceProvider.GetService<IGifRepository>();
            _scenarioContext.Add("repository", gifRepository);
            _scenarioContext.Add("client", client);
        }

        [Given(@"the user has navigated to the API endpoint ""([^""]*)""")]
        public void GivenTheUserHasNavigatedToTheAPIEndpoint(string endpoint)
        {
            _scenarioContext.Add("route", endpoint);
        }

        [Given(@"the user has some Gifs:")]
        public async Task GivenTheUserHasSomeGifs(Table table)
        {
            var repository = _scenarioContext.Get<IGifRepository>("repository");
            foreach (var row in table.Rows)
            {
                var gif = new Gif(row.GetString("Name"), new byte[] { 0x01, 0x02, 0x03 });
                await repository.InsertGifAsync(gif);
            }
        }

        [When(@"the user performs a GET request")]
        public async Task WhenTheUserPerformsAGETRequest()
        {
            var client = _scenarioContext.Get<HttpClient>("client");
            var endpoint = _scenarioContext.Get<string>("route");
            var response = await client.GetAsync(endpoint);
            _scenarioContext.Add("response", response);
        }

        [Then(@"the response should be a list of all Gif names whose name contains the string ""([^""]*)""")]
        public async Task ThenTheResponseShouldBeAListOfAllGifNamesWhoseNameContainsTheString(string input)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("response");
            var repository = _scenarioContext.Get<IGifRepository>("repository");
            var gifs = await repository.GetWhereNameContainsAsync(input);
            var result = await response.Content.ReadFromJsonAsync<List<string>>();
            result.Should().BeEquivalentTo(gifs.Select(x => x.Name));            
        }

        [Then(@"the response should be the file for the Gif named ""([^""]*)""")]
        public async Task ThenTheResponseShouldBeTheFileForTheGifNamed(string gifName)

        {
            var response = _scenarioContext.Get<HttpResponseMessage>("response");
            var repository = _scenarioContext.Get<IGifRepository>("repository");
            var gif = await repository.GetByNameAsync(gifName);
            var result = await response.Content.ReadAsByteArrayAsync();
            result.Should().BeEquivalentTo(gif.FileBytes);
        }

        [Given(@"the user has filled out the necessary Gif information in a form")]
        public void GivenTheUserHasFilledOutTheNecessaryGifInformationInAForm()
        {
            var newGifName = "newGifName";
            _scenarioContext.Add("newGifName", newGifName);
            
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(newGifName), nameof(GifCreation.Name));
            form.Add(new StreamContent(File.OpenRead(@"TestFiles/TestImage.jpg")), nameof(GifCreation.ImageFiles), "TestImage.jpg");
            form.Add(new StreamContent(File.OpenRead(@"TestFiles/TestImage2.jpg")), nameof(GifCreation.ImageFiles), "TestImage2.jpg");

            _scenarioContext.Add("form", form);
        }

        [When(@"the user performs a POST request with the form data")]
        public async Task WhenTheUserPerformsAPOSTRequestWithTheFormData()
        {
            var client = _scenarioContext.Get<HttpClient>("client");
            var endpoint = _scenarioContext.Get<string>("route");
            var form = _scenarioContext.Get<MultipartFormDataContent>("form");
            var response = await client.PostAsync(endpoint, form);
            _scenarioContext.Add("response", response);
        }

        [Then(@"the response should be a success status code")]
        public void ThenTheResponseShouldBeASuccessStatusCode()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Then(@"the new Gif should be added to the system")]
        public async Task ThenTheNewGifShouldBeAddedToTheSystem()
        {
            var repository = _scenarioContext.Get<IGifRepository>("repository");
            var newGifName = _scenarioContext.Get<string>("newGifName");
            var gif = await repository.GetByNameAsync(newGifName);
            gif.Should().NotBeNull();
        }

        [Given(@"the user has filled out the necessary Gif information in a form with an invalid file")]
        public void GivenTheUserHasFilledOutTheNecessaryGifInformationInAFormWithAnInvalidFile()
        {
            var newGifName = "newGifName";
            _scenarioContext.Add("newGifName", newGifName);

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(newGifName), nameof(GifCreation.Name));
            form.Add(new StreamContent(File.OpenRead(@"TestFiles/TestImage.jpg")), nameof(GifCreation.ImageFiles), "TestImage.jpg");
            form.Add(new StreamContent(File.OpenRead(@"TestFiles/InvalidImage.jpg")), nameof(GifCreation.ImageFiles), "InvalidImage.jpg");

            _scenarioContext.Add("form", form);
        }

        [Then(@"the response should be a bad request status code")]
        public void ThenTheResponseShouldBeABadRequestStatusCode()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Given(@"a Gif with the same name already exists in the system")]
        public async void GivenAGifWithTheSameNameAlreadyExistsInTheSystem()
        {
            var repository = _scenarioContext.Get<IGifRepository>("repository");
            var newGifName = _scenarioContext.Get<string>("newGifName");
            await repository.InsertGifAsync(new Gif(newGifName, new byte[] { 0x01, 0x02, 0x03 }));
        }

        [Then(@"the response should be a conflict status code")]
        public void ThenTheResponseShouldBeAConflictStatusCode()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }
    }
}
