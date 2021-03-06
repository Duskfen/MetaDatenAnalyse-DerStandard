//note: needs Jquery
var nodes = new Array(); //resulting articles + resulting authors
   var links = new Array();

   let limitArticles = prompt("Wie viele Artikel sollen maximal gezeigt werden? (empfohlen bis zu 2000) (default 500)") 
   limitArticles = Number(limitArticles);
   if(isNaN(limitArticles) || limitArticles < 1){
      limitArticles = 500; 
   }

   function Node(id, properties, label){
      this.id = id;
      this.properties = properties;
      this.label = label;
   }
   function Link(source, target, value=1){
      this.source = source;
      this.target = target;
      this.value = value;
   }

   function GetData(){
      
      $.when(Authors(), Articles(), Links()).fail(failed).done(function(au,ar,li){
         //console.log(au, ar, li)
         console.log("loaded Node.js Data successfully");
         //console.log(ar, au, li)

         PreProcessData(ar[0],au[0],li[0])

         return true;
      })

      function failed(){
         console.log("------ failed loding node.js data. Try to load local data -----")
         console.log("note: If you using the Webversion, this is default")
         GetDataLocal(); //If the process fails, use the Data local stored (should be the case on the web-version)
      }

      function Authors(){
         return $.ajax({
            url:"http://localhost:3000/authors",
            type:"GET"
         });
      }
      
      function Articles(){
         return $.ajax({
            url:"http://localhost:3000/articles",
            type:"GET"
         });
      }
      
      function Links(){
         return $.ajax({
            url:"http://localhost:3000/links",
            type:"GET"
         });
      }
   }

   function GetDataLocal(){
      $.when(articles(), author(), links()).fail(failed).done(function(ar,au,li){
         PreProcessData(ar[0]["records"],au[0]["records"],li[0]["records"]);
      })

      function articles(){
         return $.getJSON("Data/article.json")
      }
      function author(){
         return $.getJSON("Data/author.json");
      }
      function links(){
         return $.getJSON("Data/links.json")
      }

      function failed(){
         console.log("failed loding nodejs and local data");
         d3.select("#wrapper_top")
            .append("div").attr("id","failed")
            .append("p").text("failed loading data")
      }
   }

   function PreProcessData(articles, authors, links){

      console.log(articles)

      if(limitArticles > articles.length) document.getElementById("current_Nodes").innerText = articles.length;
      else document.getElementById("current_Nodes").innerText = limitArticles;

      document.getElementById("max_Nodes").innerText = articles.length;

      GetNodes();
      GetLinks();

      function GetNodes(){
         // console.log(articles[0]._fields[0].identity.low); //node id
         // console.log(articles[i]._fields[0].properties)  /node properties

         if (limitArticles > articles.length) limitArticles = articles.length;
         for(let i = 0; i<limitArticles; i++){
            nodes.push(new Node(articles[i]._fields[0].identity.low, articles[i]._fields[0].properties, "article"))
         }
         

         for(let i = 0; i<authors.length; i++){
            nodes.push(new Node(authors[i]._fields[0].identity.low, authors[i]._fields[0].properties, "author"))
            nodes[nodes.length-1].properties.anz_links = 15
         }

         
         // nodes.push(new Node(54, "test"))
         //  console.log(nodes)
      }
      function GetLinks(){
        //console.log(links[0]._fields) //[0].low -> source; [1].low -> target
         for(let i = 0; i<links.length; i++){
            let source = links[i]._fields[0].low;
            let target = links[i]._fields[1].low;
            if(contain_id(nodes, source) && contain_id(nodes, target)){
               this.links.push(new Link(source, target));
            }
         }

         //this is for counting the links for every author (to determine the circle size...)
         var counts = {};
         this.links.forEach(function(element) {
         counts[element.source] = (counts[element.source] || 0) + 1;
         });



         //apply each count to the right node
         console.log(counts["1676"]);
         nodes.forEach(element => {
            if(element.label == "author"){
               element.properties.anz_links = counts[element.id.toString()];
               if(element.properties.anz_links == undefined) element.properties.anz_links = 0;
            }
         });

         //remove Authors without a link
         nodes = nodes.filter(item => item.properties.anz_links !== 0);
      }

      function contain_id(items, id){
         for(let element of items){
            if(element.id == id) return true; 
         }
         return false;
      }

       DrawData();
   }

   function DrawData(){
      var data = new Array()
      data.push(nodes);
      data.push(links);

      let international =  getComputedStyle(document.getElementById("international")).backgroundColor; //used for get css colors
      let inland = getComputedStyle(document.getElementById("inland")).backgroundColor;
      let wirtschaft = getComputedStyle(document.getElementById("wirtschaft")).backgroundColor;
      let web = getComputedStyle(document.getElementById("web")).backgroundColor;
      let sport = getComputedStyle(document.getElementById("sport")).backgroundColor;
      let panorama = getComputedStyle(document.getElementById("panorama")).backgroundColor;
      let kultur= getComputedStyle(document.getElementById("kultur")).backgroundColor;
      let etat = getComputedStyle(document.getElementById("etat")).backgroundColor;
      let wissenschaft = getComputedStyle(document.getElementById("wissenschaft")).backgroundColor;
      let undefined_col = getComputedStyle(document.getElementById("undefined")).backgroundColor;

      console.log(data);

      //- start 
      const wrapper = d3.select("#wrapchart");
      const width = wrapper.node().getBoundingClientRect().width;
      const height = wrapper.node().getBoundingClientRect().height;

      console.log(wrapper.node().getBoundingClientRect().height)

      const svg = d3.select("#chart")
         .attr("width", width)
         .attr("height", height)

      const simulation = d3.forceSimulation()
         .force("charge", d3.forceManyBody().strength(-200))
         .force("x", d3.forceX(width/2))
         .force("y", d3.forceY(height/2))

      
      function getNodeColor(node){
         if(node.label=="author") return "#e55039";
         // else return "#b2bec3";
         else if(node.label=="article"){
            if(node.properties.resort=="international") return international;
            else if(node.properties.resort=="inland") return inland;
            else if(node.properties.resort=="wirtschaft") return wirtschaft;
            else if(node.properties.resort=="web") return web;
            else if(node.properties.resort=="sport") return sport;
            else if(node.properties.resort=="panorama") return panorama;
            else if(node.properties.resort=="kultur") return kultur;
            else if(node.properties.resort=="etat") return etat;
            else if(node.properties.resort=="wissenschaft") return wissenschaft;
            else return undefined_col;
         }
      }

      const linkElements = svg.append("g")
         .selectAll("line")
         .data(links)
         .enter().append("line")
            .attr("stroke-width", 3)
            .attr("stroke", "#E5E5E5")


      let min = 99;
      let max = 0;
      for(let item of nodes){
         if(item.label == "author"){
            if(min > item.properties.anz_links) min = item.properties.anz_links;
            if(max < item.properties.anz_links) max = item.properties.anz_links;
         }
      }

      var nodeScale = d3.scaleLinear()
         .domain([min, max])
         .range([15, 35])
      
      console.log(nodes)
      console.log(min);

      const nodeElements = svg.append("g")
         .selectAll("circle")
         .data(nodes)
         .enter().append("circle")
            .attr("r", function(d){
               if(d.label == "author") return nodeScale(d.properties.anz_links); 
               else return 10;
            })
            .attr("fill", getNodeColor)
            .attr("stroke", "#fff")
            .attr("stroke-width", 2)
      
      const textElements = svg.append("g")
         .selectAll("text")
         .data(nodes)
         .enter().append("text")
            .text(node => node.properties.name)
            .attr("font-size", node => (nodeScale(node.properties.anz_links)))
         
      simulation.nodes(nodes).on("tick", () => {
         nodeElements
            .attr("cx", node => node.x)
            .attr("cy", node => node.y)
         textElements
            .attr("x", node => node.x)
            .attr("y", node => node.y)
         linkElements
            .attr("x1", link => link.source.x)
            .attr("y1", link => link.source.y)
            .attr("x2", link => link.target.x)
            .attr("y2", link => link.target.y)
      })

      simulation.force("link", d3.forceLink(links)
         .id(link => link.id))
      

      //user interactions
      //drag drop
      const dragDrop = d3.drag()
         .on("start", node => {
            node.fx = node.x
            node.fy = node.y
         })
         .on("drag", node => {
            simulation.alphaTarget(0.3).restart()
            node.fx = d3.event.x
            node.fy = d3.event.y
         })
         .on("end", node => {
            if(!d3.event.active){
               simulation.alphaTarget(0)
            }
            node.fx = null
            node.fy = null
         })

         nodeElements.call(dragDrop) //add it to te nodeElements  
         
      //zoom

      let zoomBehavior = d3.zoom()
         .scaleExtent([0.05, 7])
         .on("zoom", function(){
            // console.log(d3.event.transform);
            let zoom = d3.event.transform;
            // zoom.k = zoom.k*0.2;
            d3.selectAll("#chart g").transition(d3.easeLinear).duration(250).attr("transform", zoom)
         })
      d3.select("#wrapchart").call(zoomBehavior)


      //resize
      function resize(e){
         // get width/height with container selector (body also works)
         // or use other method of calculating desired values
         var width = $('#wrapchart').width(); 
         var height = $('#wrapchart').height(); 

         // set attrs and 'resume' force 
         svg.attr('width', width);
         svg.attr('height', height);
         simulation
            .force("x", d3.forceX(width / 2))
            .force("y", d3.forceY(height / 2))
            .alpha(0.7).restart();
      }
      window.onresize = resize;

      //highlight selection
      function getNeighbors(node){
         return links.reduce((neighbors, link)=> {
            if(link.target === node.id) neighbors.push(link.source)
            else if(link.source === node.id) neighbors.push(link.target)
            return neighbors
            }, [node.id])
      }
      function isNeighborLink(node,link){
         return link.target === node.id || link.source.id === node.id
      }

      // function getTextColor(node, neighbors){
      //    // console.log(neighbors)
      //    return neighbors.indexOf(node.id) ? "black" : "black"
      // }
      function getLinkColor(node, link){
         return isNeighborLink(node, link) ? "#d63031" : "#e5e5e5"
      }
      function selectNode(selectedNode){

         const neighbors = getNeighbors(selectedNode)

         nodeElements
            .attr("fill", node => getNodeColor(node))
         // textElements
         //    .attr("fill", node => getTextColor(node, neighbors))
         linkElements
            .attr("stroke", link => getLinkColor(selectedNode, link))
      }
      nodeElements.on("mouseover", selectNode);

}
GetData();