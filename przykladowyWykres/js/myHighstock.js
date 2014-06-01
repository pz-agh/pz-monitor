function TimelineWidget(type)  {
    var data=[];
    
    Highcharts.setOptions({
        global : {
            useUTC : false
        }
    });
	
    // Create the chart
    $('#container').highcharts('StockChart', {
        chart : {
            chart: {
                renderTo: 'container',
                type: 'spline'
            },
            events : {
                load : function() {
                    loadDataToSeries(type);
                    for(var i=0; i<data.length; i++){
                        this.addSeries({
                            name: resources[i].id,
                            data: data[i]
                        });
                    }
                }
            }
        },
        
        series:{
            marker : {
                enabled : true,
                radius : 6,
                lineWidth: 1.5
            },
            cursor: 'pointer'
        },
        
        yAxis: {
            gridLineDashStyle: 'Dot'    
        },
        
        xAxis:{
            ordinal: true
        },
		
        rangeSelector: {
            buttons: [{
                count: 30,
                type: 'second',
                text: '30S'
            },{
                count: 1,
                type: 'minute',
                text: '1M'
            }, {
                count: 5,
                type: 'minute',
                text: '5M'
            }, {
                count: 30,
                type: 'minute',
                text: '30M'
            },{
                count: 1,
                type: 'hour',
                text: '1H'
            }, {
                type: 'all',
                text: 'All'
            }],
            inputEnabled: false,
            selected: 5
        },
		
        title : {
            text : type
        },
		
        exporting: {
            enabled: false
        },
		
        series : []
    });
    
    function loadDataToSeries(type){
        data = [];
        var i;
        var tmpData=[];
        
        for(i=0; i<resources.length; i++){
            var resource = resources[i];
            tmpData.length=0;
            
            for(var j=0; j<resource.measurements.length; j++){
                var measurement = resource.measurements[j];
                var date = measurement.measurement_time;
                var day=parseInt(date.substring(0,2));
                var month=parseInt(date.substring(3,5));
                var year=parseInt("20"+date.substring(6,8));
                var hour=parseInt(date.substring(9,11));
                var minutes=parseInt(date.substring(12,14));
                var seconds=parseInt(date.substring(15,17));
                
                if(type=='free_memory'){
                    tmpData.push([
                        Date.UTC(year, month-1, day, hour-2, minutes, seconds), //31-05-14:19:00:42
                        measurement.free_memory
                        ]);
                }
                else if(type=='cpu_usage'){
                    tmpData.push([
                        Date.UTC(year, month-1, day, hour-2, minutes, seconds),
                        measurement.cpu_usage
                        ]);
                }
            }
            tmpData.sort(function(a, b){
                return a[0]-b[0];
            } );
            data.push(tmpData);
        }
    }

    function sortMeasurements(a, b){
        return a[0]-b[0];
    }         
}
        
