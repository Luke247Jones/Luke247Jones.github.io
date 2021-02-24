function changeText() {
  document.getElementById("textArea").style.fontSize = "x-large";
}

function fancySchmancy() {
  document.getElementById("textArea").style.fontStyle = "italic";
  document.getElementById("textArea").style.fontWeight = "bold";
  document.getElementById("textArea").style.fontFamily = "allegro,verdana,sans-serif";
  document.getElementById("textArea").style.color = "blue";
  document.getElementById("textArea").style.textDecoration = "underline";
  alert("Text changed to Fancy Schmancy!");
}

function boringBetty() {
  document.getElementById("textArea").style.fontStyle = "initial";
  document.getElementById("textArea").style.fontWeight = "initial";
  document.getElementById("textArea").style.fontFamily = "initial";
  document.getElementById("textArea").style.color = "initial";
  document.getElementById("textArea").style.textDecoration = "initial";
  alert("Text changed to Boring Betty..");
}

function moo() {
  var str = document.getElementById("textArea").value;
  var parts = str.split(".");
  var str2 = parts.join("Moo.");
  document.getElementById("textArea").value = str2;
  document.getElementById("textArea").style.textTransform="uppercase";
}
