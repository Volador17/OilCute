function [x,r1,r2]=Mix2(x1,x2)
% �����ұ���ģ�飬�������
% x����Һ��ģ����ף�r1��r2�����ֻ�����x1, x2����Ӧ������
% x1��x2�����ֻ����ֵĹ��ף�
[m,n]=size(x1);
scale=0:2.5:100;
[m1,n1]=size(scale);
for i=1:n1
    x(:,i)=(x1*scale(i)+x2*(100-scale(i)))/100;
    %aa=rand(2002,1);aa=aa/1000000;
    % x(:,i)=x(:,i)+aa;
    r1(i)=scale(i)';r2(i)=100-scale(i)';
end
