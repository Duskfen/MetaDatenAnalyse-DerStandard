var articles
var authors
var links

async function getData(){
   const neo4j = require("neo4j-driver")

   const driver = neo4j.driver("bolt://localhost", neo4j.auth.basic("neo4j", "pass"))
   const session = driver.session()
   const excludeAuthors = 'where not author.name in ["APA", "dpa", "sid", "Reuters", "APA/dpa", "red", "APA/Reuters", "jdo", "AFP", "APA/AFP"]';
   try{
      articles = await session.run("match(author:author)-->(article) " + excludeAuthors + " return distinct article, id(article) as id")
      authors = await session.run("match(author:author)-->(article) " + excludeAuthors + " return distinct author, id(author) as id")
      links = await session.run("match(author:author)-->(article) " + excludeAuthors + " return distinct id(author), id(article)")
   }
   catch (error){
      console.log(error)
   }
   finally{
      await session.close()
   }
   
   await driver.close()
}

async function awriteToFile(){
   const fs = require("fs");
   let data = JSON.stringify(authors);
   fs.writeFileSync("Data/author.json", data);
}
async function arwriteToFile(){
   const fs = require("fs");
   let data = JSON.stringify(articles);
   fs.writeFileSync("Data/article.json", data);
}
async function lrwriteToFile(){
   const fs = require("fs");
   let data = JSON.stringify(links);
   fs.writeFileSync("Data/links.json", data);
}

async function Listen(){
   
   const express = require('express')
   const app = express()
   const port = 3000

   app.use(function(req, res, next) {
      res.header("Access-Control-Allow-Origin", "*"); // update to match the domain you will make the request from
      res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
      next();
    });
   app.get('/articles', (req, res) => res.send(articles.records))
   app.get('/authors', (req, res) => res.send(authors.records))
   app.get('/links', (req, res) => res.send(links.records))

   app.listen(port, () => console.log(`Example app listening at http://localhost:${port}`))
}
getData().then(function(){
   // awriteToFile();  //use to export
   // arwriteToFile();
   // lrwriteToFile();
   Listen()
})

