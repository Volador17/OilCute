function [x,r1,r2]=Mix2(x1,x2)
% 计算混兑比例模块，两两混兑
% x，混兑后的模拟光谱，r1，r2，两种混兑组分x1, x2的相应比例。
% x1，x2，两种混兑组分的光谱，
[m,n]=size(x1);
scale=0:2.5:100;
[m1,n1]=size(scale);
for i=1:n1
    x(:,i)=(x1*scale(i)+x2*(100-scale(i)))/100;
    %aa=rand(2002,1);aa=aa/1000000;
    % x(:,i)=x(:,i)+aa;
    r1(i)=scale(i)';r2(i)=100-scale(i)';
end
