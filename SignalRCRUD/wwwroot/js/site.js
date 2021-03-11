
$(() => {
    LoadProductData();

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();
    connection.on("LoadProducts", function () {
        LoadProductData();
    })


    LoadProductData();
    function LoadProductData() {
        var tr = '';

        $.ajax({
            url: 'Products/GetProducts',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `
                                <tr>
                                <td>${v.ProductName}</td>
                                <td>${v.Category}</td>
                                <td>${v.UnitPrice}</td>
                                <td>${v.StockQty}</td>
                                <td> 
                                <a href='../Products/Edit?id=${v.ProductId}'>Edit</a>
                                <a href='../Products/Details?id=${v.ProductId}'>Details</a>
                                <a href='../Products/Delete?id=${v.ProductId}'>Delete</a>
                                </td>
                            `
                })
                $("#tableBody").html(tr);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }
})