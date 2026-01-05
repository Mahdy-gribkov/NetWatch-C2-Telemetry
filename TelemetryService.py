import time
import grpc
import psutil
from concurrent import futures
import monitoring_pb2
import monitoring_pb2_grpc

class MonitorService(monitoring_pb2_grpc.NetworkMonitorServicer):
    def StreamLatencies(self, request, context):
        print("Telemetry Agent Started.")
        
        last_net_io = psutil.net_io_counters()
        last_time = time.time()

        while True:
            current_time = time.time()
            dt = current_time - last_time
            if dt == 0: dt = 0.001

            cpu_val = psutil.cpu_percent(interval=0.1)
            yield self._create_packet("CPU_Core", int(cpu_val), "#2196F3")

            ram_val = psutil.virtual_memory().percent
            yield self._create_packet("RAM_Module", int(ram_val), "#9C27B0")

            curr_net_io = psutil.net_io_counters()
            bytes_diff = (curr_net_io.bytes_sent + curr_net_io.bytes_recv) - \
                         (last_net_io.bytes_sent + last_net_io.bytes_recv)
            
            net_speed_kb = int(bytes_diff / dt / 1024)
            
            yield self._create_packet("Net_Traffic_KBps", net_speed_kb, "#FF9800")

            last_net_io = curr_net_io
            last_time = current_time

    def _create_packet(self, name, value, color):
        return monitoring_pb2.PingUpdate(
            target_name=name,
            latency_ms=value, 
            status_color=color,
            timestamp=time.strftime("%H:%M:%S")
        )

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    monitoring_pb2_grpc.add_NetworkMonitorServicer_to_server(MonitorService(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()

if __name__ == '__main__':
    serve()