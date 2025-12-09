// Define PID

namespace pid_class{
    public class PID{
    private double integral;
    private double prevError;

    public double Kp { get; private set; }
    public double Ki { get; private set; }
    public double Kd { get; private set; }

    public PID(double Kp, double Ki, double Kd)
    {
        this.integral = 0;
        this.prevError = 0;
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public double OutputPID(double distWall1, double distWall2, double dt)
    {
        double error = distWall1 - distWall2;
        this.integral += error * dt;
        double derivative = (error - this.prevError) / dt;

        // Anti-windup mechanism (optional)
        // Limit the integral term to prevent excessive accumulation
        double maxIntegral = 10.0;
        if (this.integral > maxIntegral)
        {
            this.integral = maxIntegral;
        }
        else if (this.integral < -maxIntegral)
        {
            this.integral = -maxIntegral;
        }

        double output = this.Kp * error + this.Ki * this.integral + this.Kd * derivative;

        this.prevError = error;
        return output;
    }
}
}