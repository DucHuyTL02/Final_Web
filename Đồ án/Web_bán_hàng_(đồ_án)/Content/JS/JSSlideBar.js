// Hiển thị hoặc ẩn chat box
function toggleChatBox() {
    const chatBox = document.getElementById('chat-box');
    if (chatBox.style.display === 'none' || chatBox.style.display === '') {
        chatBox.style.display = 'block';
    } else {
        chatBox.style.display = 'none';
    }
}


// Chuyển lên đầu trang
function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// Chuyển xuống cuối trang
function scrollToBottom() {
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
}