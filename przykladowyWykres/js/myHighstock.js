function TimelineWidget()  {

    Highcharts.setOptions({
        global : {
            useUTC : false
        }
    });
	
    // Create the chart
    $('#container').highcharts('StockChart', {
        chart : {
            events : {
                load : function() {
                    var series1 = this.series[0];
                    var series2 = this.series[1];
                    var series3 = this.series[2];
                    setInterval(function() {
                        var x = (new Date()).getTime(), // current time
                        y = Math.round(Math.random() * 100);
                        series1.addPoint([x, Math.round(Math.random() * 10)*y], true, true);
                        series2.addPoint([x, Math.round(Math.random() * 10)*y], true, true);
                        series3.addPoint([x, Math.round(Math.random() * 10)*y], true, true);
                    }, 5000);
                }
            }
        },
		
        rangeSelector: {
            buttons: [{
                count: 5,
                type: 'second',
                text: '5S'
            },{
                count: 10,
                type: 'second',
                text: '10S'
            },{
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
                type: 'all',
                text: 'All'
            }],
            inputEnabled: false,
            selected: 2
        },
		
        title : {
            text : 'Live random data'
        },
		
        exporting: {
            enabled: false
        },
		
        series : [{
            name : 'Random data 1',
            data : (function() {
                var data = [], time = (new Date()).getTime(), i;

                for( i = -999; i <= 0; i++) {
                    data.push([
                        time + i * 1000,
                        Math.round(Math.random() * 100)
                        ]);
                }
                return data;
            })()
        },{
            name : 'Random data 2',
            data : (function() {
                var data = [], time = (new Date()).getTime(), i;

                for( i = -999; i <= 0; i++) {
                    data.push([
                        time + i * 1000,
                        Math.round(Math.random() * 100)
                        ]);
                }
                return data;
            })()
        },{
            name : 'Random data 3',
            data : (function() {
                var data = [], time = (new Date()).getTime(), i;

                for( i = -999; i <= 0; i++) {
                    data.push([
                        time + i * 1000,
                        Math.round(Math.random() * 100)
                        ]);
                }
                return data;
            })()
        }]
    });

             
}
        
