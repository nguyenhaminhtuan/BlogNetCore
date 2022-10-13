﻿namespace BlogNetCore.Common.DTOs;

public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public ICollection<TagDto> Tags { get; set; }
    public AuthorDto Author { get; set; }
}