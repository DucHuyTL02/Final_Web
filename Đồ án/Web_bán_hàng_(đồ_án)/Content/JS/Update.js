function updateQuantity(id, quantity) {
    $.ajax({
        url: '/Giohang/UpdateQuantity',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (result) {
            $('.cart-items').html(result);
        }
    });
}

function removeItem(id) {
    $.ajax({
        url: '/Giohang/RemoveItem',
        type: 'POST',
        data: { id: id },
        success: function (result) {
            $('.cart-items').html(result);
        }
    });
}
