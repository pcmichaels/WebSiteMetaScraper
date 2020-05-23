# WebSiteMetaScraper
A simple library that validates a URL, and scrapes the site for metadata.

# Usage

## WebSiteMeta.Sample

See WebSiteMeta.Sample for a console app example.

Run sample with a selection of well known sites:

```
WebSiteMeta.Sample.exe -t
```

Run sample with the <a href="https://en.wikipedia.org/wiki/List_of_most_popular_websites">top 50 sites</a>:

```
WebSiteMeta.Sample.exe -50
```

Run sample for a specific site:

```
WebSiteMeta.Sample.exe -s https://pmichaels.net
```

### For Example:

```
WebSiteMeta.Sample.exe -s www.sun.com
```

The result should be:

```
--==Trying www.sun.com==--
Url is valid
Charset: utf-8
Url: https://www.oracle.com/sun/
Title: Oracle and Sun Microsystems | Strategic Acquisitions | Oracle
Description: Since Oracle acquired Sun in 2010, Oracle's hardware and software engineers have worked side-by-side to build fully integrated systems and optimized solutions.
```

Note that the URL has changed due to the redirect.


## Inside Your Web Application

### Startup.cs

```
services.AddScoped<IFindMetaData, FindMetaData>(a =>
{
    var factory = a.GetService<IHttpClientFactory>();
    var client = factory.CreateClient();
    var wrapper = new DefaultHttpClientWrapper(client);
    return new FindMetaData(wrapper);
});
```

### In your controller or service

```
public FunkyService(IFindMetaData findMetaData)
{    
    _findMetaData = findMetaData;
}
```

```
string cleanUrl = _findMetaData.CleanUrl(url);
bool isValid = _findMetaData.ValidateUrl(cleanUrl);
if (!isValid)
{
    // Error
}

var data = await _findMetaData.Run(cleanUrl);
if (!data.IsSuccess)
{                
    // Error
}

return DataResult<SiteMetaData>.Success(new SiteMetaData()
{
    // Do something with the metadata:
    // data.Metadata.Title,
    // data.Metadata.Description
});
```            


# Contribution

PRs are welcome, but please check first.
