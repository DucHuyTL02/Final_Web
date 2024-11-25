<script>
    function toggleImageOption(option) {
        if (option === 'upload') {
        document.getElementById('uploadSection').style.display = 'block';
    document.getElementById('urlSection').style.display = 'none';
        } else {
        document.getElementById('uploadSection').style.display = 'none';
    document.getElementById('urlSection').style.display = 'block';
        }
    }
</script>