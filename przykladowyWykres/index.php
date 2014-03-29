<!DOCTYPE html>
<html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
        <script src="http://code.highcharts.com/stock/highstock.js"></script>
        <script type="text/javascript" src="http://www.highcharts.com/highslide/highslide-full.min.js"></script>  
        <script type="text/javascript" src="http://www.highcharts.com/highslide/highslide.config.js" charset="utf-8"></script> 
        <script src="http://code.highcharts.com/stock/modules/exporting.js"></script>
        <link rel="stylesheet" type="text/css" href="http://www.highcharts.com/highslide/highslide.css" /> 

        <script type="text/javascript" src="js/myHighstock.js"></script>
        
        <script>
             var timelineWidget;
                
             $(document).ready(function() {                 
                timelineWidget = new TimelineWidget();  
                
            })
            
        </script>
            
            
    </head>
    <body>
        <div id="container" style="height: 500px; min-width: 500px"></div>​
         <div id="error" style="height: 500px; min-width: 500px"></div>​
                
    </body>
</html>
    