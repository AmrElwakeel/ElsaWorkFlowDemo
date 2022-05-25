﻿using Elsa.Activities.Http.Models;
using Elsa.Services;
using Elsa.Services.Models;
using ElsaWorkFlow.DomainDataBase;
using ElsaWorkFlow.DomainDataBase.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaWorkFlow.WorkflowContexts
{
    public class BlogPostWorkflowContextProvider : WorkflowContextRefresher<BlogPost>
    {
        private readonly BlogDBContext _blogDbContext;

        public BlogPostWorkflowContextProvider(BlogDBContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        /// <summary>
        /// Loads a BlogPost entity from the database.
        /// </summary>
        public override async ValueTask<BlogPost> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default)
        {
            var blogPostId = context.ContextId;
            return  _blogDbContext.BlogPosts.AsQueryable().FirstOrDefault(x => x.Id == blogPostId);
        }

        /// <summary>
        /// Updates a BlogPost entity in the database.
        /// If there's no actual workflow context, we will get it from the input. This works because we know we have a workflow that starts with an HTTP Endpoint activity that receives BlogPost models.
        /// This is a design choice for this particular demo. In real world scenarios, you might not even need this since your workflows may be dealing with existing entities, or have (other) workflows that handle initial entity creation.
        /// The key take away is: you can do whatever you want with these workflow context providers :) 
        /// </summary>
        public override async ValueTask<string> SaveAsync(SaveWorkflowContext<BlogPost> context, CancellationToken cancellationToken = default)
        {
            var blogPost = context.Context;
            var dbSet = _blogDbContext.BlogPosts;

            if (blogPost == null)
            {
                // We are handling a newly posted blog post.
                blogPost = ((HttpRequestModel)context.WorkflowExecutionContext.Input!).GetBody<BlogPost>();

                // Generate a new ID.
                blogPost.Id = Guid.NewGuid().ToString("N");

                // Set IsPublished to false to prevent caller from cheating ;)
                blogPost.IsPublished = false;

                // Set context.
                context.WorkflowExecutionContext.WorkflowContext = blogPost;
                context.WorkflowExecutionContext.ContextId = blogPost.Id;

                // Add blog post to DB.
                await dbSet.AddAsync(blogPost, cancellationToken);
            }
            else
            {
                var blogPostId = blogPost.Id;
                var existingBlogPost =  dbSet.AsQueryable().Where(x => x.Id == blogPostId).First();

                _blogDbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            }

            await _blogDbContext.SaveChangesAsync(cancellationToken);
            return blogPost.Id;
        }
    }
}
