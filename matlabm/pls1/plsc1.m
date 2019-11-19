function [w,t,p,b,my]=plsc1(x,y,k);

z=x';
my=mean(y);
y=y-my;
e=y';

for i=1:k
    ww=(e'*z/(e'*e));
    w(:,i)=(ww/norm(ww))';
    t(:,i)=z*w(:,i)/(w(:,i)'*w(:,i));
    pp=(t(:,i)'*z/(t(:,i)'*t(:,i)));
    p(:,i)=(pp/norm(pp))';
    b(i)=e'*t(:,i)/(t(:,i)'*t(:,i));
    z=z-t(:,i)*p(:,i)';
    e=e-b(i)*t(:,i);
end

