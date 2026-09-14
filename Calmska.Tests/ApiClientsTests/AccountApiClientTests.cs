using System.Text.Json;
using Calmska.ApiClients;
using Calmska.ApiClients.Clients;
using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Moq.Protected;

namespace Calmska.Tests.ApiClientsTests;

public class AccountApiClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IAccountApiClient _accountApiClient;
    public AccountApiClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)                                                      
        {                                                                                                                    
            BaseAddress = new Uri("https://api.example.com/api/v1/")                                                         
        }; 
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<HttpClientService>.Instance;                       
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var options = Microsoft.Extensions.Options.Options.Create(jsonOptions);
                                                                                                                             
        var httpClientService = new HttpClientService(httpClient, logger, options);  
        _accountApiClient = new AccountApiClient(httpClientService);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsOperationResultWithPaginatedResultOfAccountDTO_WhenSuccessAndHaveList()
    {
        var (pageNumber, pageSize) = (1, 10);
        var expectedData = new PaginatedResult<AccountDTO?>
        {
            Items = new List<AccountDTO?> { new(){ UserName = "testuser" } },
            TotalCount = 1
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().Contains($"accounts?pageNumber={pageNumber}&pageSize={pageSize}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedData)
            });
        
        var result = await _accountApiClient.GetAllAsync(pageNumber, pageSize, CancellationToken.None);

        result.Should().NotBeNull();
        result.Error.Should().BeEmpty();
        result.Result.Should().NotBeNull();
        result.Result.Items.Should().NotBeNull();
        result.Result.Items.Should().ContainSingle(x => x!.UserName == "testuser");
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsOperationResultWithEmptyListAndErrorMessage_WhenFailed()
    {
        var (pageNumber, pageSize) = (1, 10);
        string errorMessage = "Accounts not found:";
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = JsonContent.Create(errorMessage)
            });
        
        var result = await _accountApiClient.GetAllAsync(pageNumber, pageSize, CancellationToken.None);

        result.Result.Should().BeNull();
        result.Error.Should().NotBeEmpty();
        result.Error.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTrue_WhenSuccessLogin()
    {
        var loginCriteria = new LoginDTO { Email = "test@test.com", Password = "123456789" };
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains($"accounts/login")
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage{
                        StatusCode = HttpStatusCode.OK,
                        Content = JsonContent.Create(true)
                    }
                );

        var result = await _accountApiClient.LoginAsync(loginCriteria, CancellationToken.None);
        
        result.Result.Should().BeTrue();
        result.Error.Should().BeEmpty();
    }
    
    [Fact]
    public async Task LoginAsync_ReturnsFalse_WhenUnSuccessLogin()
    {
        var loginCriteria = new LoginDTO { Email = "test@test.com", Password = "badPassword" };
        string errorMessage = "Account does not exist";
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains($"accounts/login")
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage{
                    StatusCode = HttpStatusCode.NotFound,
                    Content = JsonContent.Create(errorMessage)
                }
            );

        var result = await _accountApiClient.LoginAsync(loginCriteria, CancellationToken.None);
        
        result.Result.Should().BeFalse();
        result.Error.Should().Contain(errorMessage);
    }
    
    [Fact]
    public async Task AddAsync_ReturnsError_WhenAccountWithTheEmailAddressAlreadyExist()
    {
        var loginCriteria = new AccountDTO { Email = "test@test.com", PasswordHashed = "badPassword", UserName = "Test"};
        var errorMessage = "The user with provided email exists"; 
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains("accounts")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage{
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorMessage)
                }
            );

        var result = await _accountApiClient.AddAsync(loginCriteria, CancellationToken.None);
        
        result.Result.Should().BeFalse();
        result.Error.Should().NotBeEmpty();
        result.Error.Should().Contain(errorMessage);
    }
    
    [Fact]                                                                                                                   
    public async Task UpdateAsync_ReturnsTrue_WhenUpdateIsSuccessful()                                                       
    {                                                                                                                        
        var updatedAccount = new AccountDTO { UserId = Guid.NewGuid(), UserName = "UpdatedUser" };                           
                                                                                                                             
        _mockHttpMessageHandler.Protected()                                                                                  
            .Setup<Task<HttpResponseMessage>>("SendAsync",                                                                   
                ItExpr.Is<HttpRequestMessage>(req =>                                                                         
                    req.Method == HttpMethod.Put &&                                                                          
                    req.RequestUri!.ToString().Contains("accounts")),                                                        
                ItExpr.IsAny<CancellationToken>()                                                                            
            )                                                                                                                
            .ReturnsAsync(new HttpResponseMessage                                                                            
            {                                                                                                                
                StatusCode = HttpStatusCode.OK    
            });                                                                                                              
                                                                                                                             
        var result = await _accountApiClient.UpdateAsync(updatedAccount, CancellationToken.None);                                                                             
                                                                                                                             
        result.Result.Should().BeTrue();                                                                                     
        result.Error.Should().BeEmpty();                                                                                     
    }                                                                                                                        
                                                                                                                             
    [Fact]                                                                                                                   
    public async Task UpdateAsync_ReturnsError_WhenUpdateFails()                                                             
    {                                                                                                                        
        var updatedAccount = new AccountDTO { UserId = Guid.NewGuid(), UserName = "UpdatedUser" };                           
        string errorMessage = "Failed to update account";                                                                    
                                                                                                                             
        _mockHttpMessageHandler.Protected()                                                                                  
            .Setup<Task<HttpResponseMessage>>("SendAsync",                                                                   
                ItExpr.IsAny<HttpRequestMessage>(),                                                                          
                ItExpr.IsAny<CancellationToken>()                                                                            
            )                                                                                                                
            .ReturnsAsync(new HttpResponseMessage                                                                            
            {                                                                                                                
                StatusCode = HttpStatusCode.BadRequest,                                                                      
                Content = new StringContent(errorMessage)                                                                    
            });                                                                                                              
                                                                                                                             
        var result = await _accountApiClient.UpdateAsync(updatedAccount, CancellationToken.None);                            
                                                                                                                             
        result.Result.Should().BeFalse();                                                                                    
        result.Error.Should().NotBeEmpty();                                                                                  
        result.Error.Should().Contain(errorMessage);                                                                         
    }                                                                                                                        
    
    [Fact]                                                                                                                   
    public async Task DeleteAsync_ReturnsTrue_WhenDeleteIsSuccessful()                                                       
    {                                                                                                                        
        var accountId = Guid.NewGuid();                                                                                      
                                                                                                                             
        _mockHttpMessageHandler.Protected()                                                                                  
            .Setup<Task<HttpResponseMessage>>("SendAsync",                                                                   
                ItExpr.Is<HttpRequestMessage>(req =>                                                                         
                    req.Method == HttpMethod.Delete &&                                                                       
                    req.RequestUri!.ToString().Contains($"accounts?accountId={accountId}")),                                 
                ItExpr.IsAny<CancellationToken>()                                                                            
            )                                                                                                                
            .ReturnsAsync(new HttpResponseMessage                                                                            
            {                                                                                                                
                StatusCode = HttpStatusCode.OK                                                                               
            });                                                                                                              
                                                                                                                             
        var result = await _accountApiClient.DeleteAsync(accountId, CancellationToken.None);                                 
                                                                                                                             
        result.Result.Should().BeTrue();                                                                                     
        result.Error.Should().BeEmpty();                                                                                     
    }                                                                                                                        
                                                                                                                             
    [Fact]                                                                                                                   
    public async Task DeleteAsync_ReturnsError_WhenDeleteFails()                                                             
    {                                                                                                                        
        var accountId = Guid.NewGuid();                                                                                      
        string errorMessage = "Account not found, cannot delete";                                                            
                                                                                                                             
        _mockHttpMessageHandler.Protected()                                                                                  
            .Setup<Task<HttpResponseMessage>>("SendAsync",                                                                   
                ItExpr.IsAny<HttpRequestMessage>(),                                                                          
                ItExpr.IsAny<CancellationToken>()                                                                            
            )                                                                                                                
            .ReturnsAsync(new HttpResponseMessage                                                                            
            {                                                                                                                
                StatusCode = HttpStatusCode.NotFound,                                                                        
                Content = new StringContent(errorMessage)                                                                    
            });                                                                                                              
                                                                                                                             
        var result = await _accountApiClient.DeleteAsync(accountId, CancellationToken.None);                                 
                                                                                                                             
        result.Result.Should().BeFalse();                                                                                    
        result.Error.Should().NotBeEmpty();                                                                                  
        result.Error.Should().Contain(errorMessage);                                                                         
    }             
}