const API_BASE_URL = "http://localhost:5090";
let USERNAME = "";

let currentScore = 0;
let lives = 3;
let currentPostData = null;
let gameQueue = [];
let isProcessing = false;

const ui = {
  score: document.getElementById("score-display"),
  image: document.getElementById("game-image"),
  caption: document.getElementById("caption-text"),
  lives: document.getElementById("lives-container"),
  card: document.querySelector(".post-card"),
  footer: document.getElementById("result-footer"),

  startScreen: document.getElementById("start-screen"),
  gameScreen: document.getElementById("game-screen"),
  scoreboardScreen: document.getElementById("scoreboard-screen"),

  nameInput: document.getElementById("player-name-input"),
  startBtn: document.getElementById("start-btn"),

  scoreboardName: document.getElementById("scoreboard-player-name"),
  scoreboardScore: document.getElementById("scoreboard-final-score"),
  leaderboardBody: document.getElementById("leaderboard-body"),
  playAgainBtn: document.getElementById("play-again-btn"),
};

lucide.createIcons();

ui.nameInput.addEventListener("input", (e) => {
  e.target.value = e.target.value.replace(/\s/g, "");
});

ui.startBtn.addEventListener("click", async () => {
  const enteredName = ui.nameInput.value.trim();

  if (!enteredName) {
    alert("Enter a player name to continue");
    return;
  }

  ui.startBtn.innerText = "CONNECTING...";
  ui.startBtn.disabled = true;

  try {
    const url = `${API_BASE_URL}/insta/createaccount?name=${encodeURIComponent(enteredName)}`;

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) throw new Error("Could not create account");

    USERNAME = enteredName;

    ui.startScreen.classList.add("hidden");
    ui.gameScreen.classList.remove("hidden");

    lucide.createIcons();

    initGame();
  } catch (error) {
    console.error("Account creation error:", error);
    alert("Error connecting to server.");
    ui.startBtn.innerText = "START GAME";
    ui.startBtn.disabled = false;
  }
});

async function initGame() {
  ui.caption.innerText = "Loading game data...";
  await fetchAllPosts();
  if (gameQueue.length > 0) {
    loadNextFromQueue();
  } else {
    ui.caption.innerText = "No images found.";
  }
}

async function fetchAllPosts() {
  try {
    const response = await fetch(`${API_BASE_URL}/insta`);
    const data = await response.json();

    console.log("Data received from GET /insta:", data);

    if (Array.isArray(data)) {
      gameQueue = data;
    } else {
      gameQueue = [data];
    }
  } catch (error) {
    console.error("Error fetching posts:", error);
    ui.caption.innerText = "Error connecting to server.";
  }
}

async function validateGuessWithBackend(postId, userGuessedAi) {
  try {
    const url = `${API_BASE_URL}/insta?playerId=${USERNAME}&postId=${postId}&answer=${userGuessedAi}`;

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) throw new Error("API Error");

    const isGuessCorrect = await response.json();

    return isGuessCorrect;
  } catch (error) {
    console.error("Failed to validate guess:", error);
    return false;
  }
}

function loadNextFromQueue() {
  isProcessing = false;
  ui.card.classList.remove("glow-correct", "glow-wrong");
  ui.footer.classList.add("hidden");
  ui.footer.classList.remove("text-correct", "text-wrong");

  if (gameQueue.length === 0) {
    endGame("finished");
    return;
  }

  const apiPost = gameQueue.shift();

  const imageBasePath =
    "../API/AI quizelet API/AI quizelet API/Database/mongodb-configs";

  const backendImageInfo = apiPost.image.url.startsWith("/")
    ? apiPost.image.url
    : "/" + apiPost.image.url;

  currentPostData = {
    id: apiPost.id,
    imageUrl: imageBasePath + backendImageInfo,
  };

  ui.image.src = currentPostData.imageUrl;
  ui.caption.innerText = "Is this image Real or AI generated?";
  lucide.createIcons();
}

async function handleGuess(guessInput) {
  if (isProcessing || !currentPostData) return;
  isProcessing = true;

  const userGuessedAi = guessInput === "ai";

  const isCorrect = await validateGuessWithBackend(
    currentPostData.id,
    userGuessedAi,
  );

  const actuallyAi =
    (userGuessedAi && isCorrect) || (!userGuessedAi && !isCorrect);

  if (isCorrect) {
    currentScore += 50;
  } else {
    loseLife();
  }
  ui.score.innerText = currentScore;

  updateResultUI(isCorrect, actuallyAi);

  if (lives > 0) {
    setTimeout(() => {
      loadNextFromQueue();
    }, 1500);
  }
}

function updateResultUI(isCorrect, actuallyAi) {
  const actualLabel = actuallyAi ? "AI GENERATED" : "REAL POST";
  const iconName = isCorrect ? "check" : "x";

  ui.card.classList.add(isCorrect ? "glow-correct" : "glow-wrong");

  ui.footer.innerHTML = `<i data-lucide="${iconName}"></i> ${actualLabel}`;
  ui.footer.classList.remove("hidden");

  ui.footer.classList.remove("text-correct", "text-wrong");
  ui.footer.classList.add(isCorrect ? "text-correct" : "text-wrong");

  lucide.createIcons();
}

function loseLife() {
  if (lives > 0) {
    lives--;
    const hearts = ui.lives.querySelectorAll(".heart-icon");
    if (hearts[lives]) {
      hearts[lives].classList.add("heart-lost");
      hearts[lives].style.fill = "none";
    }
  }

  if (lives === 0) {
    setTimeout(() => {
      endGame("no_lives");
    }, 500);
  }
}

// Key buttons (right/left)

document.addEventListener("keydown", (e) => {
  if (isProcessing) return;
  if (e.key === "ArrowLeft") handleGuess("real");
  else if (e.key === "ArrowRight") handleGuess("ai");
});

// SCOREBOARD

function getLeaderboard() {
  const data = localStorage.getItem("instaGameLeaderboard");
  return data ? JSON.parse(data) : [];
}

function saveScore(playerName, score) {
  const leaderboard = getLeaderboard();

  leaderboard.push({ name: playerName, score: score });
  leaderboard.sort((a, b) => b.score - a.score);
  const top5 = leaderboard.slice(0, 5);

  localStorage.setItem("instaGameLeaderboard", JSON.stringify(top5));
}

function renderLeaderboardRows(entries) {
  ui.leaderboardBody.innerHTML = "";

  if (!entries || entries.length === 0) {
    ui.leaderboardBody.innerHTML = `
      <div class="scoreboard-row">
        <div class="col-rank">-</div>
        <div class="col-name">No scores yet</div>
        <div class="col-score">-</div>
      </div>
    `;
    return;
  }

  // Render the rows
  entries.forEach((entry, idx) => {
    const name = entry.name ?? entry.playerName ?? "Unknown";
    const score = entry.score ?? 0;

    const row = document.createElement("div");
    row.className = "scoreboard-row";
    row.innerHTML = `
      <div class="col-rank">${idx + 1}</div>
      <div class="col-name">${name}</div>
      <div class="col-score">${score}</div>
    `;
    ui.leaderboardBody.appendChild(row);
  });
}

function endGame(reason = "") {
  isProcessing = true;

  ui.gameScreen.classList.add("hidden");
  ui.scoreboardScreen.classList.remove("hidden");

  ui.scoreboardName.innerText = USERNAME || "PLAYER";
  ui.scoreboardScore.innerText = currentScore;

  saveScore(USERNAME || "PLAYER", currentScore);

  renderLeaderboardRows(getLeaderboard());
}

// RESET THE GAME BUTTON

function resetGame() {
  currentScore = 0;
  lives = 3;
  currentPostData = null;
  gameQueue = [];
  isProcessing = false;

  if (ui.score) ui.score.innerText = currentScore;
  if (ui.caption) ui.caption.innerText = "Loading content...";
  if (ui.image) ui.image.src = "";

  if (ui.lives) {
    const hearts = ui.lives.querySelectorAll(".heart-icon");
    hearts.forEach((h) => {
      h.classList.remove("heart-lost");
      h.style.fill = "";
    });
  }

  if (ui.card) ui.card.classList.remove("glow-correct", "glow-wrong");
  if (ui.footer) {
    ui.footer.classList.add("hidden");
    ui.footer.classList.remove("text-correct", "text-wrong");
    ui.footer.innerHTML = "";
  }

  ui.scoreboardScreen?.classList.add("hidden");
  ui.gameScreen?.classList.add("hidden");
  ui.startScreen?.classList.remove("hidden");

  if (ui.startBtn) {
    ui.startBtn.disabled = false;
    ui.startBtn.innerText = "START GAME";
  }

  if (ui.nameInput) ui.nameInput.value = "";

  lucide.createIcons();
}

ui.playAgainBtn.addEventListener("click", () => {
  resetGame();
});

// BUTTONS from Arduino
const socket = new WebSocket("ws://127.0.0.1:8181");

socket.onopen = () => {
  console.log("Connected to Arduino via WebSocket");
};

socket.onmessage = (event) => {
  const value = event.data; // "ai" or "real"
  handleGuess(value);
};
