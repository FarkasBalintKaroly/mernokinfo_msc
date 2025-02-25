import cv2
import sys
import threading

# Accessing the camera
s = 0
if len(sys.argv) > 1:
    s = int(sys.argv[1])  # Konvertáljuk számmá

source = cv2.VideoCapture(s)
source.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
source.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

win_name = "Camera Preview"
cv2.namedWindow(win_name, cv2.WINDOW_NORMAL)

# Creating Deep Neural Network Model
protoFile = "pose_deploy_linevec_faster_4_stages.prototxt"
weightsFile = "model/pose_iter_160000.caffemodel"

nPoints = 15
POSE_PAIRS = [
    [0, 1], [1, 2], [2, 3], [3, 4], [1, 5], [5, 6], [6, 7],
    [1, 14], [14, 8], [8, 9], [9, 10], [14, 11], [11, 12], [12, 13],
]

net = cv2.dnn.readNetFromCaffe(protoFile, weightsFile)
netInputSize = (224, 224)

# Globális változók
frame = None
processed_frame = None
lock = threading.Lock()
running = True


# Frame olvasása külön szálon
def read_frames():
    global frame, running
    while running:
        has_frame, temp_frame = source.read()
        if not has_frame:
            break
        with lock:
            frame = cv2.flip(temp_frame, 1)


# Pose estimation külön szálon
def process_frames():
    global frame, processed_frame, running
    frame_skip = 2
    frame_count = 0

    while running:
        if frame is None:
            continue

        frame_count += 1
        if frame_count % frame_skip != 0:
            continue

        with lock:
            temp_frame = frame.copy()

        frame_height, frame_width = temp_frame.shape[:2]

        # Create blob
        inpBlob = cv2.dnn.blobFromImage(temp_frame, 1.0 / 255, netInputSize, (127.5, 127.5, 127.5), swapRB=True,
                                        crop=False)
        net.setInput(inpBlob)

        # Run Model
        output = net.forward()

        # Scale factors
        scaleX = frame_width / output.shape[3]
        scaleY = frame_height / output.shape[2]

        # Empty list to store the detected keypoints
        points = []
        threshold = 0.1

        for i in range(nPoints):
            probMap = output[0, i, :, :]
            _, prob, _, point = cv2.minMaxLoc(probMap)

            x, y = int(scaleX * point[0]), int(scaleY * point[1])
            points.append((x, y) if prob > threshold else None)

        # Draw skeleton
        for partA, partB in POSE_PAIRS:
            if partA < len(points) and partB < len(points):
                if points[partA] is not None and points[partB] is not None:
                    cv2.line(temp_frame, points[partA], points[partB], (255, 255, 0), 2)
                    cv2.circle(temp_frame, points[partA], 8, (255, 0, 0), thickness=-1, lineType=cv2.FILLED)

        with lock:
            processed_frame = temp_frame


# Indítsuk el a két szálat
thread1 = threading.Thread(target=read_frames, daemon=True)
thread2 = threading.Thread(target=process_frames, daemon=True)

thread1.start()
thread2.start()

# Megjelenítés főszálon
while cv2.waitKey(1) != 27:
    with lock:
        if processed_frame is not None:
            cv2.imshow(win_name, processed_frame)

# Leállítás
running = False
source.release()
cv2.destroyAllWindows()
