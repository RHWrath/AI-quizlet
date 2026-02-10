const API_BASE_URL = "http://localhost:3000";
const USERNAME = "player_1";

let currentScore = 200;
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
};

lucide.createIcons();
initGame();

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
    // MOCK MODE:
    const data = await generateMockData();

    // REAL MODE:
    /*
        const response = await fetch(`${API_BASE_URL}/insta`);
        const data = await response.json();
        */

    if (Array.isArray(data)) {
      gameQueue = data;
    } else {
      gameQueue = [data];
    }
  } catch (error) {
    console.error("Error fetching posts:", error);
  }
}

async function submitGuessToBackend(postId, userGuess) {
  try {
    const guessString = userGuess.toLowerCase();
    const url = `${API_BASE_URL}/insta?userName=${USERNAME}&postId=${postId}&guess=${guessString}`;

    console.log(`Mock submitting to: ${url}`);

    // REAL MODE:
    /*
        await fetch(url, {
            method: 'POST', 
            headers: { 'Content-Type': 'application/json' }
        });
        */
  } catch (error) {
    console.error("Failed to report guess:", error);
  }
}

function loadNextFromQueue() {
  isProcessing = false;
  ui.card.classList.remove("glow-correct", "glow-wrong");
  ui.footer.classList.add("hidden");
  ui.footer.classList.remove("text-correct", "text-wrong");

  if (gameQueue.length === 0) {
    alert("You've played all the images! Final Score: " + currentScore);
    location.reload();
    return;
  }

  const apiPost = gameQueue.shift();

  currentPostData = {
    id: apiPost.id,
    imageUrl: apiPost.image.link,
    isAi: apiPost.image.ai,
    caption: "Is this image Real or AI generated?",
  };

  ui.image.src = currentPostData.imageUrl;
  ui.caption.innerText = currentPostData.caption;
  lucide.createIcons();
}

function handleGuess(guessInput) {
  if (isProcessing || !currentPostData) return;
  isProcessing = true;

  const isUserGuessingReal = guessInput === "real";
  const isCorrect =
    (isUserGuessingReal && !currentPostData.isAi) ||
    (!isUserGuessingReal && currentPostData.isAi);

  updateResultUI(isCorrect);

  if (isCorrect) {
    currentScore += 50;
  } else {
    loseLife();
  }
  ui.score.innerText = currentScore;

  submitGuessToBackend(currentPostData.id, guessInput);

  setTimeout(() => {
    loadNextFromQueue();
  }, 2000);
}

function updateResultUI(isCorrect) {
  const actualLabel = currentPostData.isAi ? "AI GENERATED" : "REAL PHOTO";
  const iconName = isCorrect ? "check" : "x";

  ui.card.classList.add(isCorrect ? "glow-correct" : "glow-wrong");

  ui.footer.innerHTML = `<i data-lucide="${iconName}"></i> ${actualLabel}`;
  ui.footer.classList.remove("hidden");
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
      alert("GAME OVER - Final Score: " + currentScore);
      location.reload();
    }, 500);
  }
}

function generateMockData() {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve([
        {
          id: 101,
          image: {
            link: "https://picsum.photos/400?grayscale",
            ai: true,
          },
        },
        {
          id: 102,
          image: {
            link: "https://picsum.photos/400",
            ai: false,
          },
        },
        {
          id: 103,
          image: {
            link: "https://picsum.photos/401",
            ai: false,
          },
        },
        {
          id: 104,
          image: {
            link: "https://picsum.photos/401?grayscale",
            ai: true,
          },
        },
      ]);
    }, 800);
  });
}

// Key buttons (right/left)

document.addEventListener("keydown", (e) => {
  if (e.key === "ArrowLeft") handleGuess("real");
  else if (e.key === "ArrowRight") handleGuess("ai");
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
