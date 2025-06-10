

/*< --Render Chart using Chart.js -->*/
window.renderChart = (canvasId, chartData) => {
    const ctx = document.getElementById(canvasId).getContext('2d');
    new Chart(ctx, chartData);
};

/* Bar Chart  */
function renderChart(chartType, labels, dataValues, labelTitle, bgColors, bdrColors) {
    const ctx = document.getElementById('myChart');

    new Chart(ctx, {
        type: chartType,
        data: {
            labels: labels,
            datasets: [{
                label: labelTitle,
                data: dataValues,
                backgroundColor: bgColors,
                borderColor: bdrColors,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}


/* Pie Chart */
